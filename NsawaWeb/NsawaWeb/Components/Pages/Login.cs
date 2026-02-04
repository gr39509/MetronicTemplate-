using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using NsawaWeb.Services.Services;

namespace NsawaWeb.Components.Pages;

public partial class Login
{
    [Inject] private AuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ApiClient ApiClient { get; set; } = default!;

    [SupplyParameterFromForm]
    private LoginDto request { get; set; } = new LoginDto();
    
    private bool authChecked = false;

    
    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string errorMessage = string.Empty;
    private string infoMessage = string.Empty;
    private bool isLoading = false;
    private bool showPasswordError = false;

    protected override async Task OnInitializedAsync()
    {
        if (await AuthService.IsAuthenticatedAsync())
        {
            NavigationManager.NavigateTo(ReturnUrl ?? "/home");
            return;
        }

        authChecked = true;
    }

    private string GetPasswordClass()
    {
        return showPasswordError ? "form-control is-invalid" : "form-control";
    }

    private async Task HandleLogin()
    {
        // Reset errors
        showPasswordError = false;
        errorMessage = string.Empty;
        infoMessage = string.Empty;
        
        // Validate input
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            errorMessage = "Phone number is required.";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            showPasswordError = true;
            errorMessage = "Password is required.";
            return;
        }

        if (request.PhoneNumber.Length != 10)
        {
            errorMessage = "Phone number must be exactly 10 digits.";
            return;
        }

        isLoading = true;
        StateHasChanged();

        try
        {
            var result = await ApiClient.LoginAsync(body: request);
            
            if (result.Success && result.Data != null && !string.IsNullOrEmpty(result.Data.Token))
            {
                await AuthService.LoginAsync(result.Data.Token, request.PhoneNumber);
            
                NavigationManager.NavigateTo(ReturnUrl ?? "/home", forceLoad: true);
            }
            else
            {
                errorMessage = result.Message ?? "Login failed. Please check your credentials and try again.";
            }
        }
        catch (ApiException apiEx)
        {
            errorMessage = apiEx.StatusCode switch
            {
                401 => "Invalid phone number or password.",
                400 => "Please check your input and try again.",
                403 => "Access denied. Please contact support.",
                500 => "Server error. Please try again later.",
                200 => "Invalid login attempt.",
                _ => $"An error occurred during login. Please try again. (Error code: {apiEx.StatusCode})"
            };
        }
        catch (HttpRequestException httpEx)
        {
            errorMessage = "Unable to connect to the server. Please check your internet connection.";
        }
        catch (Exception ex)
        {
            errorMessage = "An unexpected error occurred. Please try again.";
            Console.WriteLine($"Login error: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
    
    
    private void ClearError()
    {
        errorMessage = string.Empty;
        showPasswordError = false;
    }
}