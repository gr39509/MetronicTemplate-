using System.Net.Http.Headers;

namespace NsawaWeb.Services.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthHeaderHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            
            if (!string.IsNullOrEmpty(token))
            {
               
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                //Console.WriteLine("No token found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding auth header");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}