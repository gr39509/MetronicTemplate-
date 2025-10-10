// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using Microsoft.AspNetCore.Components.Authorization;
//
// namespace NovaAccounts.Services.AuthServices;
//
// public class CustomAuthStateProvider : AuthenticationStateProvider
// {
//     private readonly AuthService _authService;
//     private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
//
//     public CustomAuthStateProvider(AuthService authService)
//     {
//         _authService = authService;
//     }
//
//     public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//         try
//         {
//             var token = await _authService.GetAccessTokenAsync();
//             
//             if (string.IsNullOrEmpty(token))
//             {
//                 return new AuthenticationState(_anonymous);
//             }
//
//             var claims = ParseClaimsFromJwt(token);
//             var identity = new ClaimsIdentity(claims, "jwt");
//             var user = new ClaimsPrincipal(identity);
//
//             return new AuthenticationState(user);
//         }
//         catch
//         {
//             return new AuthenticationState(_anonymous);
//         }
//     }
//
//     public void NotifyUserAuthentication(string token)
//     {
//         var claims = ParseClaimsFromJwt(token);
//         var identity = new ClaimsIdentity(claims, "jwt");
//         var user = new ClaimsPrincipal(identity);
//
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//     }
//
//     public void NotifyUserLogout()
//     {
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
//     }
//
//     private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
//     {
//         var handler = new JwtSecurityTokenHandler();
//         var token = handler.ReadJwtToken(jwt);
//         return token.Claims;
//     }
// }


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace NovaAccounts.Services.AuthServices;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            Console.WriteLine("🔑 [AuthStateProvider] GetAuthenticationStateAsync called");
            
            var tokenResult = await _localStorage.GetAsync<string>("accessToken");
            
            Console.WriteLine($"🔑 [AuthStateProvider] Token retrieval - Success: {tokenResult.Success}");
            
            if (!tokenResult.Success || string.IsNullOrEmpty(tokenResult.Value))
            {
                Console.WriteLine("🔑 [AuthStateProvider] No token found, returning anonymous");
                return new AuthenticationState(_anonymous);
            }

            Console.WriteLine($"🔑 [AuthStateProvider] Token found (length: {tokenResult.Value.Length})");

            var claims = ParseClaimsFromJwt(tokenResult.Value);
            var claimsList = claims.ToList();
            
            // Add the token as a claim so AuthHeaderHandler can access it
            claimsList.Add(new Claim("token", tokenResult.Value));
            
            var identity = new ClaimsIdentity(claimsList, "jwt");
            var user = new ClaimsPrincipal(identity);
            
            Console.WriteLine($"🔑 [AuthStateProvider] Authenticated user: {identity.Name ?? "No name"}, IsAuthenticated: {identity.IsAuthenticated}");
            
            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [AuthStateProvider] Exception: {ex.GetType().Name} - {ex.Message}");
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task NotifyUserAuthentication(string token)
    {
        try
        {
            Console.WriteLine($"🔐 [AuthStateProvider] NotifyUserAuthentication called with token length: {token?.Length ?? 0}");
            
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("❌ [AuthStateProvider] Token is null or empty!");
                return;
            }

            // Store token
            await _localStorage.SetAsync("accessToken", token);
            Console.WriteLine("✅ [AuthStateProvider] Token stored in localStorage");
            
            // Verify it was stored
            var verifyResult = await _localStorage.GetAsync<string>("accessToken");
            Console.WriteLine($"✅ [AuthStateProvider] Verification - Token stored: {verifyResult.Success}");
            
            var claims = ParseClaimsFromJwt(token);
            var claimsList = claims.ToList();
            claimsList.Add(new Claim("token", token));
            
            var identity = new ClaimsIdentity(claimsList, "jwt");
            var user = new ClaimsPrincipal(identity);

            Console.WriteLine($"✅ [AuthStateProvider] Created identity - IsAuthenticated: {identity.IsAuthenticated}");
            Console.WriteLine($"✅ [AuthStateProvider] Claims count: {claimsList.Count}");

            var authState = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
            
            Console.WriteLine("✅ [AuthStateProvider] NotifyAuthenticationStateChanged called");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [AuthStateProvider] Exception in NotifyUserAuthentication: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    public async Task NotifyUserLogout()
    {
        try
        {
            Console.WriteLine("🚪 [AuthStateProvider] NotifyUserLogout called");
            
            await _localStorage.DeleteAsync("accessToken");
            await _localStorage.DeleteAsync("refreshToken");
            
            Console.WriteLine("✅ [AuthStateProvider] Tokens deleted");
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            
            Console.WriteLine("✅ [AuthStateProvider] Logged out successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [AuthStateProvider] Exception in NotifyUserLogout: {ex.Message}");
        }
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            
            Console.WriteLine($"🔍 [AuthStateProvider] JWT Claims:");
            foreach (var claim in token.Claims)
            {
                Console.WriteLine($"   - {claim.Type}: {claim.Value}");
            }
            
            return token.Claims;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [AuthStateProvider] Error parsing JWT: {ex.Message}");
            return Enumerable.Empty<Claim>();
        }
    }
}