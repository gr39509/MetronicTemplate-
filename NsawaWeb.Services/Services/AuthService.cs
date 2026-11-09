using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NsawaWeb.Services.Services;

/// <summary>
/// Holds the signed-in user's JWT for the current circuit.
/// The token is persisted through ITokenStorage (browser localStorage on the web, secure storage in the app) and cached in memory.
/// This class never navigates; callers decide where to go after sign-in or sign-out.
/// </summary>
public class AuthService
{
    private const string TokenKey = "authToken";
    private const string UserNameKey = "userName";
    private const string FullNameKey = "fullName";

    private readonly ITokenStorage _storage;
    private readonly ILogger<AuthService> _logger;
    private string? _cachedToken;
    private ClaimsPrincipal? _cachedUser;
    private DateTime? _tokenExpiryUtc;
    private bool _storageLoaded;
    private CustomAuthStateProvider? _authStateProvider;

    public AuthService(ITokenStorage storage, ILogger<AuthService> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    internal void SetAuthStateProvider(CustomAuthStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!_storageLoaded)
        {
            _cachedToken = await ReadStorageAsync(TokenKey);
            _tokenExpiryUtc = _cachedToken is null ? null : ReadExpiry(_cachedToken);
            _storageLoaded = true;
        }

        if (_cachedToken is null || _tokenExpiryUtc is null || _tokenExpiryUtc <= DateTime.UtcNow)
        {
            return null;
        }

        return _cachedToken;
    }

    public async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var token = await GetTokenAsync();
        if (token is null)
        {
            _cachedUser = null;
            return null;
        }

        if (_cachedUser is not null)
        {
            return _cachedUser;
        }

        try
        {
            var claims = ParseClaimsFromJwt(token).ToList();
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var displayName = await ReadStorageAsync(FullNameKey) ?? await ReadStorageAsync(UserNameKey);
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    claims.Add(new Claim(ClaimTypes.Name, displayName));
                }
            }

            _cachedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return _cachedUser;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Stored token could not be parsed");
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetUserAsync();
        return user?.Identity?.IsAuthenticated == true;
    }

    public async Task LoginAsync(string token, string userName, string? fullName = null)
    {
        await _storage.SetAsync(TokenKey, token);
        await _storage.SetAsync(UserNameKey, userName);
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            await _storage.SetAsync(FullNameKey, fullName);
        }

        _cachedToken = token;
        _tokenExpiryUtc = ReadExpiry(token);
        _cachedUser = null;
        _storageLoaded = true;
        _authStateProvider?.NotifyAuthenticationStateChanged();
    }

    public async Task<string?> GetDisplayNameAsync()
    {
        var user = await GetUserAsync();
        var claim = user?.FindFirst(ClaimTypes.Name)
                    ?? user?.FindFirst("name")
                    ?? user?.FindFirst("fullName")
                    ?? user?.FindFirst("full_name");
        return string.IsNullOrWhiteSpace(claim?.Value) ? await ReadStorageAsync(UserNameKey) : claim.Value;
    }

    public async Task LogoutAsync()
    {
        foreach (var key in new[] { TokenKey, UserNameKey, FullNameKey })
        {
            try
            {
                await _storage.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not clear {Key} from storage", key);
            }
        }

        _cachedToken = null;
        _cachedUser = null;
        _tokenExpiryUtc = null;
        _storageLoaded = true;
        _authStateProvider?.NotifyAuthenticationStateChanged();
    }

    private Task<string?> ReadStorageAsync(string key) => _storage.GetAsync(key);

    private static DateTime? ReadExpiry(string token)
    {
        try
        {
            var exp = ParseClaimsFromJwt(token).FirstOrDefault(c => c.Type == "exp");
            return exp is null ? null : DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp.Value)).UtcDateTime;
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var json = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ParseBase64WithoutPadding(payload));
        if (json is null)
        {
            yield break;
        }

        foreach (var (key, value) in json)
        {
            if (value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in value.EnumerateArray())
                {
                    yield return new Claim(key, item.ToString());
                }
            }
            else
            {
                yield return new Claim(key, value.ToString());
            }
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
