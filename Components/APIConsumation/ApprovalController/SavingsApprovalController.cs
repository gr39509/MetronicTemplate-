using Microsoft.AspNetCore.Mvc;
using NovaAccounts.SharedModels.ApiService;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;


    [ApiController]
    [Route("api/[controller]")]
    public class SavingsApprovalController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SavingsApprovalController> _logger;

        public SavingsApprovalController(IHttpClientFactory httpClientFactory, ILogger<SavingsApprovalController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("validate-and-approve")]
        public async Task<IActionResult> ValidateAndApproveSavingsAccount(string accountNumber)
        {
            try
            {
                _logger.LogInformation("🔍 API called with accountNumber: {AccountNumber}", accountNumber);

                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    return BadRequest("Account number is required.");
                }

                accountNumber = accountNumber.Trim();

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

                var approveModel = new ApproveSavingsAccountModel
                {
                    AccountNumber = new List<string> { accountNumber }
                };

                _logger.LogInformation("📞 Calling ApproveSavingsAccountAsync...");
                
                var result = await apiClient.ApproveSavingsAccountAsync(
                    bP_Tenant: null,
                    accept_Language: null,
                    body: approveModel
                );

                if (result?.Payload.Successful == 200)
                {
                    _logger.LogInformation("✅ Savings account approved successfully");
                    return Ok("success - Savings account approved successfully!");
                }
                else
                {
                    _logger.LogWarning("❌ Failed to approve account: {Error}", result?.Payload.ErrorMessage);
                    return BadRequest(result.Payload.ErrorMessage ?? "Failed to approve account.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error in ValidateAndApproveSavingsAccount");
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
