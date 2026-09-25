using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace NsawaWeb.Services.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly AuthService _authService;
    private readonly ILogger<CustomAuthStateProvider> _logger;

    public CustomAuthStateProvider(AuthService authService, ILogger<CustomAuthStateProvider> logger)
    {
        _authService = authService;
        _logger = logger;
        _authService.SetAuthStateProvider(this);
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var user = await _authService.GetUserAsync();
            return user is null ? Anonymous : new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not resolve authentication state");
            return Anonymous;
        }
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
