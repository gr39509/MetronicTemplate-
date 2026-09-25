using System.Net.Http.Headers;

namespace NsawaWeb.Services.Services;

/// <summary>Adds the signed-in user's bearer token to every API request.</summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthHeaderHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authService.GetTokenAsync();
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
