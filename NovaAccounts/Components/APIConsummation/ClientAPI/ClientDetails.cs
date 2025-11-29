using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NovaAccounts.Components.APIConsummation.EmailProviderConfigurationsAPI;
using NovaAccounts.Components.APIConsummation.OtpProviderConfigurationsAPI;
using NovaAccounts.Components.APIConsummation.PaymentProviderConfigurationsAPI;
using NovaAccounts.Components.APIConsummation.SMSProviderConfigurationsAPI;
using NovaAccounts.SharedModels.ApiService;
using NovaAccounts.SharedModels.Charts.DonutChart;
using NovaAccounts.SharedModels.Charts.LineChart;


namespace NovaAccounts.Components.APIConsummation.ClientAPI;

public partial class ClientDetails
{
     [Parameter]
    public Guid ClientId { get; set; }

    private EmailProviderConfigurationCreateModal emailConfigCreateModal = null!;
    private EmailProviderConfigurationEditModal emailConfigEditModal = null!;
    private OtpProviderConfigurationCreateModal otpConfigCreateModal = null!;
    private OtpProviderConfigurationEditModal otpConfigEditModal = null!;
    private PaymentProviderConfigurationCreateModal paymentConfigCreateModal = null!;
    private PaymentProviderConfigurationEditModal paymentConfigEditModal = null!;
    private SmsProviderConfigurationCreateModal smsConfigCreateModal = null!;
    private SmsProviderConfigurationEditModal smsConfigEditModal = null!;
    private SmsSendModal smsSendModal = null!;
    private EmailSendModal emailSendModal = null!;
    private PaymentDebitModal paymentDebitModal = null!;
    private PaymentCreditModal paymentCreditModal = null!;
    private OtpTestModal otpTestModal = null!;

    private ClientViewModel? clientDetails;
    private bool isLoading = false;
    private bool isLoadingEmailConfigs = false;
    private bool isLoadingSmsConfigs = false;
    private bool isLoadingOtpConfigs = false;
    private bool isLoadingPaymentConfigs = false;
    private bool isLoadingTransactions = false;
    private string errorMessage = string.Empty;
    private string transactionsErrorMessage = string.Empty;
    private string activeTab = "email";

    private List<EmailClientProviderConfigurationViewModel> emailConfigurations = new();
    private List<SMSClientProviderConfigurationViewModel> smsConfigurations = new();
    private List<OtpClientProviderConfigurationViewModel> otpConfigurations = new();
    private List<PaymentClientProviderConfigurationViewModel> paymentConfigurations = new();
    
    // Transactions
    private PaymentRequestViewModelPagedResult? transactions;
    private DateTimeOffset? transactionsStartDate = DateTimeOffset.Now.AddDays(-30);
    private DateTimeOffset? transactionsEndDate = DateTimeOffset.Now;
    private int currentTransactionPage = 1;
    private int transactionPageSize = 10;
    
    // Chart Data
    private List<LineChartData> transactionTrendsData = new();
    private List<DonutChartData> providerDistributionData = new();
    private List<DonutChartData> transactionStatusData = new();
    private List<LineChartData> monthlyPerformanceData = new();
    private bool hasTransactionData = false;

    // Color palettes
    private List<string> providerColors = new() { "#3598DC", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#e83e8c", "#20c997", "#fd7e14" };
    private List<string> statusColors = new() { "#ffc107", "#28a745", "#dc3545", "#6c757d", "#17a2b8" };

    // Statistics
    private int totalTransactions = 0;
    private decimal successRate = 0;
    private decimal totalAmount = 0;
    private decimal totalAmountPaid = 0;
    private decimal totalAmountFailed = 0;
    private int activeProviders = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadClientData();
        
        
    }


    
    // For now I will comment this out in case of loading issues I will uncomment it.
    // protected override async Task OnParametersSetAsync()
    // {
    //     await LoadClientDetailsAsync();
    // }

    private string GetTabClass(string tabName)
    {
        return activeTab == tabName ? "active bg-success text-white" : "";
    }

    private void SwitchTab(string tabName)
    {
        if (activeTab != tabName)
        {
            activeTab = tabName;
            Console.WriteLine($"Switched to tab: {tabName}");
            
            if (tabName == "transactions" && clientDetails != null)
            {
                _ = LoadTransactionsAsync();
            }
            
            StateHasChanged();
        }
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/", forceLoad:true);
    }

    private async Task LoadClientData()
    {
        try
        {
            isLoading = true;
            errorMessage = string.Empty;
            StateHasChanged();

            await LoadClientDetailsAsync();
            if (string.IsNullOrEmpty(errorMessage))
            {
                await LoadTransactionAnalytics();
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Failed to load client data. Please try again.";
            Console.WriteLine($"Error loading client data: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadClientDetailsAsync()
    {
        try
        {
            isLoading = true;
            errorMessage = string.Empty;
            StateHasChanged();

            Console.WriteLine($"Loading client details for ID: {ClientId}");

            var result = await ApiClient.ClientAsync(ClientId);

            if (result?.Success == true && result.Data != null)
            {
                clientDetails = result.Data;
                Console.WriteLine($"Loaded client: {clientDetails.Name}");

                await Task.WhenAll(
                    LoadEmailConfigurations(clientDetails.Identifier),
                    LoadSmsConfigurations(clientDetails.Identifier),
                    LoadOtpConfigurations(clientDetails.Identifier),
                    LoadPaymentConfigurations(clientDetails.Identifier),
                    LoadTransactionsAsync()
                );
                
                if (string.IsNullOrEmpty(errorMessage))
                {
                    await LoadTransactionAnalytics();
                }
            }
            else
            {
                errorMessage = result?.Message ?? "Failed to load client details.";
                Console.WriteLine($"API returned unsuccessful: {errorMessage}");
            }
        }
        catch (ApiException ex)
        {
            errorMessage = $"Server error ({ex.StatusCode}). Please try again.";
            Console.WriteLine($"API Exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading client details: {ex.Message}";
            Console.WriteLine($"Error loading client details: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadTransactionAnalytics()
    {
        try
        {
            var startDate = DateTimeOffset.Now.AddMonths(-6);
            var endDate = DateTimeOffset.Now;

            var transactionsResult = await ApiClient.TransactionsAsync(
                clientCodes: clientDetails?.Identifier,
                startDate: startDate,
                endDate: endDate,
                providerId: null,
                page: 1,
                size: 10000
            );

            if (transactionsResult?.Success == true && transactionsResult.Data?.Data != null)
            {
                var transactions = transactionsResult.Data.Data.ToList();
                
                if (transactions.Any())
                {
                    hasTransactionData = true;
                    await GenerateChartData(transactions);
                    CalculateStatistics(transactions);
                }
                else
                {
                    hasTransactionData = false;
                }
            }
            else
            {
                hasTransactionData = false;
            }
        }
        catch (ApiException apiEx)
        {
            Console.WriteLine($"API Exception loading transactions: {apiEx.Message}");
            hasTransactionData = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transaction analytics: {ex.Message}");
            hasTransactionData = false;
        }
    }

    private async Task GenerateChartData(List<PaymentRequestViewModel> transactions)
    {
        try
        {
            // Generate transaction trends data (last 6 months)
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Reverse()
                .ToList();

            transactionTrendsData = last6Months.Select(month => 
            {
                var monthStr = month.ToString("MMM yyyy");
                var monthTransactions = transactions.Where(t => 
                    t.CreatedAt.Year == month.Year && t.CreatedAt.Month == month.Month).ToList();
                
                return new LineChartData
                {
                    Year = monthStr,
                    A = monthTransactions.Count,
                    B = (decimal)monthTransactions.Sum(t => t.Amount)
                };
            }).ToList();

            // Generate provider distribution data - count all provider types
            var allProviders = new List<string>();
            
            // Add email providers
            if (emailConfigurations.Any())
                allProviders.AddRange(emailConfigurations.Select(e => e.Provider.Name));
            
            // Add SMS providers
            if (smsConfigurations.Any())
                allProviders.AddRange(smsConfigurations.Select(s => s.Provider.Name));
            
            // Add OTP providers
            if (otpConfigurations.Any())
                allProviders.AddRange(otpConfigurations.Select(o => o.Provider.Name));
            
            // Add payment providers
            if (paymentConfigurations.Any())
                allProviders.AddRange(paymentConfigurations.Select(p => p.Provider.Name));

            providerDistributionData = allProviders
                .GroupBy(p => p)
                .Select(g => new DonutChartData
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .OrderByDescending(g => g.Value)
                .Take(8)
                .ToList();

            // Generate transaction status data
            var statusGroups = transactions
                .GroupBy(t => string.IsNullOrEmpty(t.Status) ? "Unknown" : t.Status)
                .Select(g => new DonutChartData
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .ToList();

            transactionStatusData = statusGroups;

            // Generate monthly performance data (success vs failed)
            monthlyPerformanceData = last6Months.Select(month => 
            {
                var monthStr = month.ToString("MMM yyyy");
                var monthTransactions = transactions.Where(t => 
                    t.CreatedAt.Year == month.Year && t.CreatedAt.Month == month.Month).ToList();
                
                return new LineChartData
                {
                    Year = monthStr,
                    A = monthTransactions.Count(t => 
                        t.Status?.ToLower() == "success" || t.Status?.ToLower() == "completed"),
                    B = monthTransactions.Count(t => 
                        t.Status?.ToLower() == "failed" || t.Status?.ToLower() == "error")
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating chart data: {ex.Message}");
        }
    }

    private void CalculateStatistics(List<PaymentRequestViewModel> transactions)
    {
        try
        {
            totalTransactions = transactions.Count;
            
            var successfulTransactions = transactions.Count(t => 
                t.Status?.ToLower() == "success" || t.Status?.ToLower() == "completed");
            successRate = totalTransactions > 0 ? (decimal)successfulTransactions / totalTransactions : 0;
            
            var paidTransactions = transactions.Where(t => 
                t.Status?.ToLower() == "paid");
            var failedTransactions = transactions.Where(t => 
                t.Status?.ToLower() == "failed");
            totalAmountPaid = (decimal)paidTransactions.Sum(t => t.Amount);
            totalAmountFailed = (decimal)failedTransactions.Sum(t => t.Amount);
            totalAmount = (decimal)transactions.Sum(t => t.Amount);
            
            // Count all active providers from all service types
            activeProviders = emailConfigurations.Count(e => e.IsActive) +
                            smsConfigurations.Count(s => s.IsActive) +
                            otpConfigurations.Count(o => o.IsActive) +
                            paymentConfigurations.Count(p => p.IsActive);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating statistics: {ex.Message}");
        }
    }

    private async Task LoadTransactionsAsync()
    {
        if (clientDetails == null) return;

        try
        {
            isLoadingTransactions = true;
            transactionsErrorMessage = string.Empty;
            StateHasChanged();

            Console.WriteLine($"Loading transactions for client: {clientDetails.Identifier}");

            var result = await ApiClient.ClientTransactionsAsync(
                clientDetails.Identifier,
                transactionsStartDate,
                transactionsEndDate,
                null,
                currentTransactionPage,
                transactionPageSize
            );

            if (result?.Success == true && result.Data != null)
            {
                transactions = result.Data;
                Console.WriteLine($"Loaded {transactions.Data?.Count ?? 0} transactions");
            }
            else
            {
                transactionsErrorMessage = result?.Message ?? "No transactions found.";
                transactions = null;
                Console.WriteLine($"No transactions loaded: {transactionsErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            transactionsErrorMessage = $"Error loading transactions: {ex.Message}";
            Console.WriteLine($"Error loading transactions: {ex.Message}");
            transactions = null;
        }
        finally
        {
            isLoadingTransactions = false;
            StateHasChanged();
        }
    }

    private async Task RefreshTransactions()
    {
        currentTransactionPage = 1;
        await LoadTransactionsAsync();
    }

    private async Task ChangeTransactionPage(int page)
    {
        try
        {
            Console.WriteLine($"ChangeTransactionPage called with page: {page}");

            if (page < 1) page = 1;
            if (transactions != null && page > transactions.TotalPages)
                page = transactions.TotalPages;

            if (currentTransactionPage == page) return;

            Console.WriteLine($"Changing from page {currentTransactionPage} to page {page}");
            currentTransactionPage = page;

            StateHasChanged();
            await Task.Delay(10);
            await LoadTransactionsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ChangeTransactionPage: {ex.Message}");
        }
    }

    private async Task OnTransactionsDateChange()
    {
        currentTransactionPage = 1;
        await LoadTransactionsAsync();
    }

    private string GetStatusBadge(string status)
    {
        return status?.ToLower() switch
        {
            "success" or "completed" => "bg-success",
            "pending" => "bg-warning",
            "failed" or "error" => "bg-danger",
            _ => "bg-success"
        };
    }

    private string GetTransactionTypeBadge(string transactionType)
    {
        return transactionType?.ToLower() switch
        {
            "debit" => "badge-danger",
            "credit" => "badge-success",
            _ => "badge-secondary"
        };
    }


    private async Task LoadEmailConfigurations(string clientIdentifier)
    {
        try
        {
            isLoadingEmailConfigs = true;
            StateHasChanged();

            var result = await ApiClient.ConfigurationsPerClientAsync(clientIdentifier);

            if (result?.Success == true && result.Data != null)
            {
                emailConfigurations = result.Data
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.IsDefault)
                    .ThenBy(c => c.ProviderName)
                    .ToList();
            }
            else
            {
                emailConfigurations = new List<EmailClientProviderConfigurationViewModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading email configurations: {ex.Message}");
            emailConfigurations = new List<EmailClientProviderConfigurationViewModel>();
        }
        finally
        {
            isLoadingEmailConfigs = false;
            StateHasChanged();
        }
    }

    private async Task LoadSmsConfigurations(string clientIdentifier)
    {
        try
        {
            isLoadingSmsConfigs = true;
            StateHasChanged();

            var result = await ApiClient.ConfigurationsPerClient4Async(clientIdentifier);

            if (result?.Success == true && result.Data != null)
            {
                smsConfigurations = result.Data
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.IsDefault)
                    .ThenBy(c => c.ProviderName)
                    .ToList();
            }
            else
            {
                smsConfigurations = new List<SMSClientProviderConfigurationViewModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading SMS configurations: {ex.Message}");
            smsConfigurations = new List<SMSClientProviderConfigurationViewModel>();
        }
        finally
        {
            isLoadingSmsConfigs = false;
            StateHasChanged();
        }
    }

    private async Task LoadOtpConfigurations(string clientIdentifier)
    {
        try
        {
            isLoadingOtpConfigs = true;
            StateHasChanged();

            var result = await ApiClient.ConfigurationsPerClient2Async(clientIdentifier);

            if (result?.Success == true && result.Data != null)
            {
                otpConfigurations = result.Data
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.IsDefault)
                    .ThenBy(c => c.ProviderName)
                    .ToList();
            }
            else
            {
                otpConfigurations = new List<OtpClientProviderConfigurationViewModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading OTP configurations: {ex.Message}");
            otpConfigurations = new List<OtpClientProviderConfigurationViewModel>();
        }
        finally
        {
            isLoadingOtpConfigs = false;
            StateHasChanged();
        }
    }

    private async Task LoadPaymentConfigurations(string clientIdentifier)
    {
        try
        {
            isLoadingPaymentConfigs = true;
            StateHasChanged();

            var result = await ApiClient.ConfigurationsPerClient3Async(clientIdentifier);
            if (result?.Success == true && result.Data != null)
            {
                paymentConfigurations = result.Data.ToList();
            }
            else
            {
                paymentConfigurations = new List<PaymentClientProviderConfigurationViewModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading payment configurations: {ex.Message}");
            paymentConfigurations = new List<PaymentClientProviderConfigurationViewModel>();
        }
        finally
        {
            isLoadingPaymentConfigs = false;
            StateHasChanged();
        }
    }

    // Modal opening methods
    private async Task OpenSendEmailModal(EmailClientProviderConfigurationViewModel config)
    {
        if (clientDetails != null && emailSendModal != null)
        {
            await emailSendModal.OpenModalWithConfiguration(config);
        }
    }

    private async Task OpenSendSmsModal(SMSClientProviderConfigurationViewModel config)
    {
        if (clientDetails != null && smsSendModal != null)
        {
            await smsSendModal.OpenModalWithConfiguration(config);
        }
    }

    private async Task OpenPaymentDebitModal(PaymentClientProviderConfigurationViewModel config)
    {
        if (clientDetails != null && paymentDebitModal != null)
        {
            await paymentDebitModal.OpenModalWithConfiguration(config);
        }
    }

    private async Task OpenPaymentCreditModal(PaymentClientProviderConfigurationViewModel config)
    {
        if (clientDetails != null && paymentCreditModal != null)
        {
            await paymentCreditModal.OpenModalWithConfiguration(config);
        }
    }

    private async Task OpenOtpTestModal(OtpClientProviderConfigurationViewModel config)
    {
        if (clientDetails != null && otpTestModal != null)
        {
            await otpTestModal.OpenModalWithConfiguration(config);
        }
    }

    private async Task OpenCreateSmsConfiguration()
    {
        try
        {
            if (clientDetails == null || smsConfigCreateModal == null) return;
            await smsConfigCreateModal.OpenModalWithClient(clientDetails.Identifier);
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OpenCreateSmsConfiguration: {ex.Message}");
        }
    }

    private async Task OpenEditEmailConfiguration(Guid configId)
    {
        if (emailConfigEditModal != null) await emailConfigEditModal.OpenModal(configId);
    }

    private async Task OpenEditSmsConfiguration(Guid configId)
    {
        if (smsConfigEditModal != null) await smsConfigEditModal.OpenModal(configId);
    }

    private async Task OpenCreateOtpConfiguration()
    {
        if (clientDetails != null && otpConfigCreateModal != null)
            await otpConfigCreateModal.OpenModalWithClient(clientDetails.Identifier);
    }

    private async Task OpenEditOtpConfiguration(Guid configId)
    {
        if (otpConfigEditModal != null) await otpConfigEditModal.OpenModal(configId);
    }

    private async Task OpenCreatePaymentConfiguration()
    {
        if (clientDetails != null && paymentConfigCreateModal != null)
            await paymentConfigCreateModal.OpenModalWithClient(clientDetails.Identifier);
    }

    private async Task OpenEditPaymentConfiguration(Guid configId)
    {
        if (paymentConfigEditModal != null) await paymentConfigEditModal.OpenModal(configId);
    }

    // Event handlers
    private async Task HandleEmailSent() => Console.WriteLine("Email sent successfully");
    private async Task HandleSmsSent() => Console.WriteLine("SMS sent successfully");
    private async Task HandlePaymentProcessed()
    {
        Console.WriteLine("Payment processed successfully");
        if (activeTab == "transactions") await LoadTransactionsAsync();
    }
    private async Task HandleOtpTested() => Console.WriteLine("OTP tested successfully");

    private async Task HandleEmailConfigurationCreated(EmailClientProviderConfigurationViewModel createdConfig)
    {
        Console.WriteLine($"Email configuration created: {createdConfig.Description}");
        if (clientDetails != null) await LoadEmailConfigurations(clientDetails.Identifier);
    }

    private async Task HandleEmailConfigurationUpdated(EmailClientProviderConfigurationViewModel updatedConfig)
    {
        Console.WriteLine($"Email configuration updated: {updatedConfig.Description}");
        if (clientDetails != null) await LoadEmailConfigurations(clientDetails.Identifier);
    }

    private async Task HandleSmsConfigurationCreated(SMSClientProviderConfigurationViewModel createdConfig)
    {
        Console.WriteLine($"SMS configuration created: {createdConfig.Description}");
        if (clientDetails != null) await LoadSmsConfigurations(clientDetails.Identifier);
    }

    private async Task HandleSmsConfigurationUpdated(SMSClientProviderConfigurationViewModel updatedConfig)
    {
        Console.WriteLine($"SMS configuration updated: {updatedConfig.Description}");
        if (clientDetails != null) await LoadSmsConfigurations(clientDetails.Identifier);
    }

    private async Task HandleOtpConfigurationCreated(OtpClientProviderConfigurationViewModel createdConfig)
    {
        Console.WriteLine($"OTP configuration created: {createdConfig.Description}");
        if (clientDetails != null) await LoadOtpConfigurations(clientDetails.Identifier);
    }

    private async Task HandleOtpConfigurationUpdated(OtpClientProviderConfigurationViewModel updatedConfig)
    {
        Console.WriteLine($"OTP configuration updated: {updatedConfig.Description}");
        if (clientDetails != null) await LoadOtpConfigurations(clientDetails.Identifier);
    }

    private async Task HandlePaymentConfigurationCreated(PaymentClientProviderConfigurationViewModel createdConfig)
    {
        Console.WriteLine($"Payment configuration created: {createdConfig.Description}");
        if (clientDetails != null) await LoadPaymentConfigurations(clientDetails.Identifier);
    }

    private async Task HandlePaymentConfigurationUpdated(PaymentClientProviderConfigurationViewModel updatedConfig)
    {
        Console.WriteLine($"Payment configuration updated: {updatedConfig.Description}");
        if (clientDetails != null) await LoadPaymentConfigurations(clientDetails.Identifier);
    }

    // Chart rendering callbacks
    private async Task OnTransactionTrendsRendered() => Console.WriteLine("Transaction trends chart rendered successfully!");
    private async Task OnProviderDistributionRendered() => Console.WriteLine("Provider distribution chart rendered successfully!");
    private async Task OnTransactionStatusRendered() => Console.WriteLine("Transaction status chart rendered successfully!");
    private async Task OnMonthlyPerformanceRendered() => Console.WriteLine("Monthly performance chart rendered successfully!");

    // Custom classes for payment configuration deserialization
    private class CustomPaymentApiResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<CustomPaymentConfig>? Data { get; set; }
    }

    private class CustomPaymentConfig
    {
        public Guid PaymentProviderConfigurationId { get; set; }
        public bool IsDefault { get; set; }
        public Guid ClientProviderConfigurationId { get; set; }
        public string? Description { get; set; }
        public string? ClientIdentifier { get; set; }
        public Guid ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? Url { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public List<CustomCredential>? ClientProviderCredentials { get; set; }
    }

    private class CustomCredential
    {
        public Guid ClientProviderCredentialId { get; set; }
        public Guid ClientProviderConfigurationId { get; set; }
        public CustomCredentialType? CredentialType { get; set; }
        public string? CredentialKey { get; set; }
    }

    private class CustomCredentialType
    {
        public string? CredentialTypeName { get; set; }
        public string? DisplayName { get; set; }
        public bool Active { get; set; }
    }
}