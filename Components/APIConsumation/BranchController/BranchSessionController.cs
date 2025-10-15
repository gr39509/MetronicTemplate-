using Microsoft.AspNetCore.Mvc;

namespace NovaAccounts.Components.APIConsumation.BranchController;

using Microsoft.AspNetCore.Mvc;
using NovaAccounts.SharedModels.ApiService;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using NovaAccounts.SharedModels.ApiService;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BranchSessionController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BranchSessionController> _logger;

    public BranchSessionController(IHttpClientFactory httpClientFactory, ILogger<BranchSessionController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

        [HttpGet("open")]
        public async Task<IActionResult> OpenSesion(string branchCode)
        {
            try
            {
                _logger.LogInformation("🔍 API called with branch code: {BranchCode}", branchCode);

                if (string.IsNullOrWhiteSpace(branchCode))
                {
                    return BadRequest("Branch code is required.");
                }

                branchCode = branchCode.Trim();

                // Get the bearer token from the current request
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("❌ No authorization header found");
                    return Unauthorized("Authentication required.");
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                _logger.LogInformation("✅ Token found in request");

                // Create HttpClient with the token
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.BaseAddress = new Uri("http://185.132.37.188:8070");

                var apiClient = new ApiClient(httpClient, "http://185.132.37.188:8070");

                var openModel = new OpenBranchSessionCommand
                {
                    BranchCode = branchCode
                };

                _logger.LogInformation("📞 Calling OpenSessionAsync...");
                
                var result = await apiClient.OpenBranchSessionAsync(
                    bP_Tenant: null,
                    accept_Language: null,
                    body: openModel
                );

                if (result?.Status == 200)
                {
                    _logger.LogInformation("✅ Opened session successfully");
                    return Ok("success - Opened session successfully!");
                }
                else
                {
                    _logger.LogWarning("❌ Failed to open session.");
                    return BadRequest("Failed to open session.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error in OpenSesion");

                
                var errorMessage = ex.Message;

               
                if (errorMessage.Contains("Branch with code"))
                {
                    errorMessage = "The branch code you entered does not exist. Please verify and try again.";
                }
                else if (errorMessage.Contains("invalid or missing parameters"))
                {
                    errorMessage = "Some required information is missing or invalid.";
                }

                return BadRequest(new
                {
                    Message = errorMessage,
                    Details = "Please review your input and try again."
                });
            }
        }
        
        
        
        [HttpGet("close")]
        public async Task<IActionResult> CloseSesion(string branchCode)
        {
            try
            {
                _logger.LogInformation("🔍 API called with branch code: {BranchCode}", branchCode);

                if (string.IsNullOrWhiteSpace(branchCode))
                {
                    return BadRequest("Branch code is required.");
                }

                branchCode = branchCode.Trim();

                // Get the bearer token from the current request
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("❌ No authorization header found");
                    return Unauthorized("Authentication required.");
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                _logger.LogInformation("✅ Token found in request");

                // Create HttpClient with the token
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.BaseAddress = new Uri("http://185.132.37.188:8070");

                var apiClient = new ApiClient(httpClient, "http://185.132.37.188:8070");

                var closeModel = new CloseBranchSessionCommand()
                {
                    BranchCode = branchCode
                };

                _logger.LogInformation("📞 Calling OpenBranchSessionAsync...");
                
                var result = await apiClient.CloseBranchSessionAsync(
                    bP_Tenant: null,
                    accept_Language: null,
                    body: closeModel
                );

                if (result?.Status == 200)
                {
                    _logger.LogInformation("✅ Closed branch successfully");
                    return Ok("success - Closed branch successfully!");
                }
                else
                {
                    _logger.LogWarning("❌ Failed to close branch:");
                    return BadRequest("Failed to close branch.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error in OpenSesion");

                // Try to give a clear and user-friendly response
                var errorMessage = ex.Message;

                // Optionally detect if it's one of your known API-style messages
                if (errorMessage.Contains("Branch with code"))
                {
                    errorMessage = "The branch code you entered does not exist. Please verify and try again.";
                }
                else if (errorMessage.Contains("invalid or missing parameters"))
                {
                    errorMessage = "Some required information is missing or invalid.";
                }

                return BadRequest(new
                {
                    Message = errorMessage,
                    Details = "Please review your input and try again."
                });
            }
        }

}