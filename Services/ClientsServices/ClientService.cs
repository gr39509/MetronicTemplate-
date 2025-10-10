using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NovaAccounts.Services.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using NovaAccounts.Services.AuthServices;
using NovaAccounts.Services.Models.ClientModels;

namespace NovaAccounts.Services.ClientsServices
{
    public class ClientService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly ILogger<ClientService> _logger;

        public ClientService(HttpClient httpClient, AuthService authService, ILogger<ClientService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        // -------------------- Get Clients --------------------
        public async Task<List<Client>> GetClientsAsync()
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<Client>>>("/api/Clients/GetClients");

                if (response?.Payload != null && response.Status == 200)
                    return response.Payload;

                _logger.LogWarning("API returned unexpected result or null payload");
                return new List<Client>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching clients from API");
                throw new Exception("Unable to connect to the server. Please check your connection.", ex);
            }
        }

        // -------------------- Add Client --------------------
        public async Task<ApiResponse<Client?>> AddClientAsync(string clientName, bool returnID = true)
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var request = new
                {
                    clientName,
                    returnID
                };

                var response = await _httpClient.PostAsJsonAsync("/api/Clients/AddClient", request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error creating client: {Error}", errorContent);
                    return new ApiResponse<Client?>
                    {
                        Status = (int)response.StatusCode,
                        Type = "Error",
                        Instance = "/api/Clients/AddClient",
                        Payload = null
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<Client?>>();

                // If API returned null, fallback with generic response
                return result ?? new ApiResponse<Client?>
                {
                    Status = (int)response.StatusCode,
                    Type = "NoContent",
                    Instance = "/api/Clients/AddClient",
                    Payload = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception adding client");
                throw new Exception("Unable to add client. Please try again later.", ex);
            }
        }
        
        // -------------------- Get CBS Clients --------------------
        public async Task<List<ClientCBS>> GetCBSClientAsync()
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                // Expect ApiResponse<List<ClientCBS>> since payload is an array
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ClientCBS>>>("/api/Clients/GetCBSClients");

                if (response?.Payload != null && response.Status == 200)
                {
                    return response.Payload;
                }

                _logger.LogWarning("API returned unexpected result or null payload for CBS clients");
                return new List<ClientCBS>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching CBS clients from API");
                throw new Exception("Unable to connect to the server. Please check your connection.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception fetching CBS clients");
                throw new Exception("Unable to load CBS client information.", ex);
            }
        }
        
        
        // -------------------- Get CBS Clients with Savings Accounts --------------------
        public async Task<List<ClientCBS>> GetCBSClientsWithSavingsAsync()
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                // ✅ Correctly map payload as a list
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ClientCBS>>>(
                    "/api/Clients/GetCBSClientsWithSavingsAccounts"
                );

                if (response?.Payload != null && response.Status == 200)
                    return response.Payload;

                _logger.LogWarning("API returned unexpected result or null payload for CBS clients with savings accounts");
                return new List<ClientCBS>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching CBS clients with savings accounts from API");
                throw new Exception("Unable to connect to the server. Please check your connection.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception fetching CBS clients with savings accounts");
                throw new Exception("Unable to load CBS client information.", ex);
            }
        }
        
        public async Task<List<ClientBalanceModel>> GetClientsAlongWithBalancesAsync()
        {
            try
            {
                var token = await _authService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ClientBalanceModel>>>("/api/Clients/GetClientsAlongWithBalances");

                if (response?.Payload != null && response.Status == 200)
                    return response.Payload;

                _logger.LogWarning("API returned unexpected result or null payload for GetClientsAlongWithBalances");
                return new List<ClientBalanceModel>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching clients along with balances from API");
                throw new Exception("Unable to connect to the server. Please check your connection.", ex);
            }
        }



        public async Task<ApiResponse<ClientCBS>> GetCBSClientAsync(string clientNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://185.132.37.188:8070/api/Clients/GetCBSClient?clientNumber={clientNumber}");

                if (!response.IsSuccessStatusCode)
                    return new ApiResponse<ClientCBS> { Status = (int)response.StatusCode, Payload = null };

                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponse<ClientCBS>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new ApiResponse<ClientCBS> { Status = 500, Payload = null };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching CBS client: {ex.Message}");
                return new ApiResponse<ClientCBS> { Status = 500, Payload = null };
            }
        }



    }
}



// public class ClientService
// {
//     private readonly HttpClient _httpClient;
//     private readonly ILogger<ClientService> _logger;
//
//     public ClientService(HttpClient httpClient, ILogger<ClientService> logger)
//     {
//         _httpClient = httpClient;
//         _logger = logger;
//     }
//
//     public async Task<List<Client>> GetClientsAsync()
//     {
//         try
//         {
//             var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<Client>>>("/api/Clients/GetClients");
//                 
//             if (response?.Payload != null && response.Status == 200)
//             {
//                 return response.Payload;
//             }
//
//             _logger.LogWarning("API returned non-200 status or null payload");
//             return new List<Client>();
//         }
//         catch (HttpRequestException ex)
//         {
//             _logger.LogError(ex, "Error fetching clients from API");
//             throw new Exception("Unable to connect to the server. Please check your connection.", ex);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Unexpected error while fetching clients");
//             throw new Exception("An unexpected error occurred while fetching clients.", ex);
//         }
//     }
//
//     public async Task<Client?> GetClientByIdAsync(int id)
//     {
//         try
//         {
//             var response = await _httpClient.GetFromJsonAsync<ApiResponse<Client>>($"/api/Clients/GetClient/{id}");
//                 
//             if (response?.Payload != null && response.Status == 200)
//             {
//                 return response.Payload;
//             }
//
//             return null;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error fetching client {ClientId}", id);
//             throw;
//         }
//     }
// }