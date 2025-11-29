using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace NovaAccounts.Services
{
    public interface IAuthenticationService
    {
        Task<bool> IsAuthenticated();
        Task<string?> GetCurrentUsername();
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly NavigationManager _navigationManager;

        public AuthenticationService(
            ProtectedSessionStorage sessionStorage,
            NavigationManager navigationManager)
        {
            _sessionStorage = sessionStorage;
            _navigationManager = navigationManager;
        }

        public async Task<bool> IsAuthenticated()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<bool>("IsAuthenticated");
                return result.Success && result.Value;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetCurrentUsername()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>("Username");
                return result.Success ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task Logout()
        {
            await _sessionStorage.DeleteAsync("IsAuthenticated");
            await _sessionStorage.DeleteAsync("Username");
            _navigationManager.NavigateTo("/login", true);
        }
    }
}