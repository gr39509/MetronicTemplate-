using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NovaAccounts.Services.Models;

namespace NovaAccounts.Services.AuthServices;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";

    public AuthService(HttpClient httpClient, ProtectedLocalStorage localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Account/Login", loginRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                
                if (result?.Payload?.AuthResponse != null)
                {
                    await _localStorage.SetAsync(AccessTokenKey, result.Payload.AuthResponse.AccessToken);
                    await _localStorage.SetAsync(RefreshTokenKey, result.Payload.AuthResponse.RefreshToken);
                    
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", result.Payload.AuthResponse.AccessToken);
                }
                
                return result;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/Account/Logout", null);
            
            await _localStorage.DeleteAsync(AccessTokenKey);
            await _localStorage.DeleteAsync(RefreshTokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout error: {ex.Message}");
            await _localStorage.DeleteAsync(AccessTokenKey);
            await _localStorage.DeleteAsync(RefreshTokenKey);
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<string>(AccessTokenKey);
            return result.Success && !string.IsNullOrEmpty(result.Value);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<string>(AccessTokenKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task RestoreAuthenticationAsync()
    {
        var token = await GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}