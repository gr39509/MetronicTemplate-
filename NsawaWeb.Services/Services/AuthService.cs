using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace NsawaWeb.Services.Services;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthService> _logger;
    private readonly NavigationManager _navigationManager;
    private string? _cachedToken;
    private ClaimsPrincipal? _cachedUser;
    private DateTime? _tokenExpiryTime;
    private CustomAuthStateProvider? _authStateProvider;
    private readonly UpdateDeviceTokenDto  _updateDeviceToken;

    public AuthService(IJSRuntime jsRuntime, ILogger<AuthService> logger, NavigationManager navigationManager)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
        _navigationManager = navigationManager;
    }

    public void SetAuthStateProvider(CustomAuthStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    public async Task<string?> GetTokenAsync()
    {
       
        if (!string.IsNullOrEmpty(_cachedToken) && IsTokenCacheValid())
        {
            return _cachedToken;
        }

        try
        {
            _cachedToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            
          
            if (!string.IsNullOrEmpty(_cachedToken))
            {
                UpdateTokenExpiry(_cachedToken);
            }
            
            return _cachedToken;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error getting token");
            return null;
        }
    }
    
    public async Task<string?> GetStoredUserName()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "userName");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stored username");
            return null;
        }
    }

    public async Task<ClaimsPrincipal?> GetUserAsync()
    {
        // Return cached user if token is still valid
        if (_cachedUser != null && IsTokenCacheValid())
        {
            return _cachedUser;
        }

        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        try
        {
            // Check if token is expired
            if (!await IsTokenValidAsync())
            {
                //_logger.LogWarning("Token is expired");
                await ClearAuthenticationAsync();
                return null;
            }

            var claims = ParseClaimsFromJwt(token);
            var claimsList = claims.ToList();
            claimsList.Add(new Claim("token", token));
            
            var identity = new ClaimsIdentity(claimsList, "jwt");
            _cachedUser = new ClaimsPrincipal(identity);
            return _cachedUser;
        }
        catch (Exception ex)
        {
           // _logger.LogError(ex, "Error parsing user from token");
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
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userName", userName);
        
            // Store full name if provided
            if (!string.IsNullOrEmpty(fullName))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "fullName", fullName);
            }
        
            _cachedToken = token;
            UpdateTokenExpiry(token);
        
            var claims = ParseClaimsFromJwt(token);
            var claimsList = claims.ToList();
            claimsList.Add(new Claim("token", token));
        
            if (!claimsList.Any(c => c.Type == ClaimTypes.Name))
            {
                // Use full name if available, otherwise use username
                var displayName = fullName ?? userName;
                claimsList.Add(new Claim(ClaimTypes.Name, displayName));
            }
        
            var identity = new ClaimsIdentity(claimsList, "jwt");
            _cachedUser = new ClaimsPrincipal(identity);
        
            _authStateProvider?.NotifyAuthenticationStateChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during login");
            throw;
        }
    }

    // public async Task LoginAsync(string token, string userName)
    // {
    //     try
    //     {
    //         await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
    //         await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userName", userName);
    //         
    //         _cachedToken = token;
    //         UpdateTokenExpiry(token);
    //         
    //         var claims = ParseClaimsFromJwt(token);
    //         var claimsList = claims.ToList();
    //         claimsList.Add(new Claim("token", token));
    //         
    //         if (!claimsList.Any(c => c.Type == ClaimTypes.Name))
    //         {
    //             claimsList.Add(new Claim(ClaimTypes.Name, userName));
    //         }
    //         
    //         var identity = new ClaimsIdentity(claimsList, "jwt");
    //         _cachedUser = new ClaimsPrincipal(identity);
    //         
    //         _authStateProvider?.NotifyAuthenticationStateChanged();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError("Error during login");
    //         throw;
    //     }
    // }
    
    public async Task<string?> GetFullName()
    {
        try
        {
            var user = await GetUserAsync();
            if (user != null)
            {
                // Check for common name claims
                var nameClaim = user.FindFirst(ClaimTypes.Name) 
                                ?? user.FindFirst("name") 
                                ?? user.FindFirst("fullName")
                                ?? user.FindFirst("full_name");
            
                if (nameClaim != null && !string.IsNullOrEmpty(nameClaim.Value))
                {
                    return nameClaim.Value;
                }
            }
        
            // Fallback to localStorage
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "fullName");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting full name");
            return null;
        }
    }

    public async Task LogoutAsync(ApiClient? apiClient = null)
    {
        try
        {
           
            if (apiClient != null)
            {
                try
                {
                    await apiClient.UpdateDeviceTokenAsync(_updateDeviceToken);
                    _logger.LogInformation("logout successful");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("logout failed, continuing with local logout");
                }
            }

            await ClearAuthenticationAsync();
            
            _logger.LogInformation("User logged out locally");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during logout");
        }
    }

    private async Task ClearAuthenticationAsync()
    {
        // Clear local storage
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userName");
        
        _cachedToken = null;
        _cachedUser = null;
        _tokenExpiryTime = null;
        
        // Notify authentication state changed
        _authStateProvider?.NotifyAuthenticationStateChanged();
        
        // Navigate to login page
        _navigationManager.NavigateTo("/", true);
    }
    
    public async Task<bool> IsTokenValidAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return false;
    
        try
        {
            var claims = ParseClaimsFromJwt(token);
            var expiryClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expiryClaim == null) return false;
        
            // Convert Unix timestamp to DateTime
            var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(expiryClaim.Value)).UtcDateTime;
        
            return expiryDateTime > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    private void UpdateTokenExpiry(string token)
    {
        try
        {
            var claims = ParseClaimsFromJwt(token);
            var expiryClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expiryClaim != null)
            {
                _tokenExpiryTime = DateTimeOffset.FromUnixTimeSeconds(
                    long.Parse(expiryClaim.Value)).UtcDateTime;
            }
        }
        catch (Exception ex)
        {
           
            _tokenExpiryTime = null;
        }
    }

    private bool IsTokenCacheValid()
    {
        if (_tokenExpiryTime == null) return false;
        
      
        return _tokenExpiryTime.Value.AddMinutes(-5) > DateTime.UtcNow;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            claims.AddRange(keyValuePairs.Select(kvp => 
                new Claim(kvp.Key, kvp.Value?.ToString() ?? "")));
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}