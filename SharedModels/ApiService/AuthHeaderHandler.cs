using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthHeaderHandler(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the auth state
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            
            // Check if user is authenticated and get token from claims
            if (user.Identity?.IsAuthenticated == true)
            {
                var token = user.FindFirst("token")?.Value;
                
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"🔑 Adding Bearer token to request: {request.RequestUri}");
                    // Add the authorization header
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    Console.WriteLine("❌ No token found in claims for authenticated user");
                }
            }
            else
            {
                Console.WriteLine("❌ User is not authenticated");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error adding auth header: {ex.Message}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}