using Microsoft.AspNetCore.Components;
using NovaAccounts.Services.AuthServices;
using NovaAccounts.SharedModels.ApiService;

namespace NovaAccounts.Components.Dashboard.Account;

public partial class Login
{
    [SupplyParameterFromForm]
    private LoginCommand loginCommand { get; set; } = new LoginCommand();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string errorMessage = string.Empty;
    private bool isLoading = false;
    private bool showEmailError = false;
    private bool showPasswordError = false;
    
    protected override  async void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            NavigationManager.NavigateTo(GetRedirectUrl());
        }
    }

    private async Task HandleLogin()
    {
        Console.WriteLine("🔐 Starting login process...");

        // Reset error states
        showEmailError = false;
        showPasswordError = false;
        errorMessage = string.Empty;

        // Simple client-side validation
        if (!ValidateForm())
        {
            return;
        }

        isLoading = true;
        StateHasChanged();

        try
        {
            Console.WriteLine($"📤 Calling API with email: {loginCommand.Email}");

            var result = await ApiClient.LoginAsync(
                bP_Tenant: null,
                accept_Language: null,
                body: loginCommand
            );

            Console.WriteLine($"✅ API Response received. Has token: {!string.IsNullOrEmpty(result?.Payload?.AuthResponse?.AccessToken)}");

            if (result?.Payload?.AuthResponse?.AccessToken != null)
            {
                Console.WriteLine("🔑 Storing token and notifying auth provider...");

                // Notify CustomAuthStateProvider
                if (AuthStateProvider is CustomAuthStateProvider customProvider)
                {
                    await customProvider.NotifyUserAuthentication(result.Payload.AuthResponse.AccessToken);
                    Console.WriteLine("✅ Auth provider notified");
                }

                // Small delay to ensure state propagation
                await Task.Delay(100);

                // Determine redirect URL
                string redirectUrl;
                if (result.Payload.RequiresTwoFactor)
                {
                    redirectUrl = string.IsNullOrEmpty(ReturnUrl)
                        ? "/two-factor"
                        : $"/two-factor?ReturnUrl={Uri.EscapeDataString(ReturnUrl)}";
                    Console.WriteLine($"➡️ Redirecting to two-factor: {redirectUrl}");
                }
                else
                {
                    redirectUrl = GetRedirectUrl();
                    Console.WriteLine($"➡️ Redirecting to: {redirectUrl}");
                }

                // Force navigation with forceLoad
                NavigationManager.NavigateTo(redirectUrl, forceLoad: true);
            }
            else
            {
                Console.WriteLine("❌ No valid token in response");
                errorMessage = "Invalid email or password. Please try again.";
            }
        }
        catch (ApiException<ApiErrorResponse> apiEx)
        {
            Console.WriteLine($"❌ API Exception: {apiEx.Result?.ErrorMessage}");
            errorMessage = apiEx.Result?.ErrorMessage ?? "An error occurred during login.";
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            Console.WriteLine($"❌ Bad request: {ex.Message}");
            errorMessage = "Invalid request. Please check your input.";
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            Console.WriteLine($"❌ Unauthorized: {ex.Message}");
            errorMessage = "Invalid email or password.";
        }
        catch (ApiException ex) when  (ex.StatusCode == 422)
        {
            Console.WriteLine($"❌ API Exception: {ex.StatusCode} - {ex.Message}");
            errorMessage = $"Invalid email or password.";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ HTTP Request Exception: {ex.Message}");
            errorMessage = "Unable to connect to the server. Please check your connection.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            errorMessage = $"An unexpected error occurred: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private bool ValidateForm()
    {
        bool isValid = true;

        // Email validation
        if (string.IsNullOrWhiteSpace(loginCommand.Email))
        {
            showEmailError = true;
            isValid = false;
        }
        else if (!loginCommand.Email.Contains("@"))
        {
            showEmailError = true;
            errorMessage = "Please enter a valid email address.";
            isValid = false;
        }

        // Password validation
        if (string.IsNullOrWhiteSpace(loginCommand.Password))
        {
            showPasswordError = true;
            isValid = false;
        }
        else if (loginCommand.Password.Length < 4)
        {
            showPasswordError = true;
            errorMessage = "Password must be at least 4 characters.";
            isValid = false;
        }

        return isValid;
    }

    private string GetRedirectUrl()
    {
        return !string.IsNullOrEmpty(ReturnUrl) ? ReturnUrl : "/";
    }
}