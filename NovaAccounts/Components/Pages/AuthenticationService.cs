using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace NovaAccounts.Services
{ 
    using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace NovaAccounts.Services
{
    public interface IAuthenticationService
    {
        Task<bool> IsAuthenticated();
        Task<string?> GetCurrentUsername();
        Task<string?> GetCurrentFullName();
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ProtectedLocalStorage _localStorage;
        private readonly NavigationManager _navigationManager;

        public AuthenticationService(
            ProtectedSessionStorage sessionStorage,
            ProtectedLocalStorage localStorage,
            NavigationManager navigationManager)
        {
            _sessionStorage = sessionStorage;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        public async Task<bool> IsAuthenticated()
        {
            try
            {
                var result = await _localStorage.GetAsync<bool>("IsAuthenticated");
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
                var result = await _localStorage.GetAsync<string>("Username");
                return result.Success ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> GetCurrentFullName()
        {
            try
            {
                var result = await _localStorage.GetAsync<string>("FullName");
                return result.Success ? result.Value : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task Logout()
        {
            await _localStorage.DeleteAsync("IsAuthenticated");
            await _localStorage.DeleteAsync("Username");
            await _localStorage.DeleteAsync("FullName");
            _navigationManager.NavigateTo("/login", true);
        }
    }
}
}