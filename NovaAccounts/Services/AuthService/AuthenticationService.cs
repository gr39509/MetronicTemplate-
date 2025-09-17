using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NovaAccounts.Components.Dashboard.Account;
using NovaAccounts.Models.Account;


namespace NovaAccounts.Services.AuthService;

public class AuthenticationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public event Action<ClaimsPrincipal>? UserChanged;
    private ClaimsPrincipal? currentUser;

    public AuthenticationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public ClaimsPrincipal CurrentUser
    {
        get => currentUser ?? new ClaimsPrincipal();
        set
        {
            currentUser = value;
            UserChanged?.Invoke(currentUser);
        }
    }

    public async Task<Login.ApiResponse?> LoginAsync(LoginModel loginModel)
    {
        var httpclient = _httpClientFactory.CreateClient("Auth");

        var loginData = new
        {
            email = loginModel.Email?.Trim(),
            password = loginModel.Password?.Trim()
        };

        var response = await httpclient.PostAsJsonAsync("/api/Account/Login", loginData);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Login.ApiResponse>();
    }
}



public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private AuthenticationState authenticationState;

    public CustomAuthenticationStateProvider(AuthenticationService service)
    {
        authenticationState = new AuthenticationState(service.CurrentUser);

        service.UserChanged += (newUser) =>
        {
            authenticationState = new AuthenticationState(newUser);

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(newUser)));
        };
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(authenticationState);
}


public class TokenProvider
{
    public string? accesstoken { get; set; }

    public string? refreshtoken { get; set; }
}

public class UserProfile
{
    public string? UserName { get; set; }
    //other claims....
}