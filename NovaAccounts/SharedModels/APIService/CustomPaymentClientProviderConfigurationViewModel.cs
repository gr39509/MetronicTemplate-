using Newtonsoft.Json;

namespace NovaAccounts.SharedModels.ApiService;

// Custom models that match the actual API response
public class CustomPaymentClientProviderConfigurationViewModel
{
    [JsonProperty("paymentProviderConfigurationId")]
    public Guid PaymentProviderConfigurationId { get; set; }

    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; }

    [JsonProperty("clientProviderConfigurationId")]
    public Guid ClientProviderConfigurationId { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("clientIdentifier")]
    public string ClientIdentifier { get; set; } = string.Empty;

    [JsonProperty("providerId")]
    public Guid ProviderId { get; set; }

    [JsonProperty("providerName")]
    public string? ProviderName { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("provider")]
    public object? Provider { get; set; } // Handle null provider

    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    [JsonProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("createdBy")]
    public string CreatedBy { get; set; } = string.Empty;

    [JsonProperty("updatedAt")]
    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonProperty("updatedBy")]
    public string? UpdatedBy { get; set; }

    [JsonProperty("clientProviderCredentials")]
    public List<CustomClientProviderCredentialViewModel> ClientProviderCredentials { get; set; } = new();
}

public class CustomClientProviderCredentialViewModel
{
    [JsonProperty("clientProviderCredentialId")]
    public Guid ClientProviderCredentialId { get; set; }

    [JsonProperty("clientProviderConfigurationId")]
    public Guid ClientProviderConfigurationId { get; set; }

    [JsonProperty("credentialType")]
    public CustomCredentialTypeViewModel CredentialType { get; set; } = new();

    [JsonProperty("credentialKey")]
    public string CredentialKey { get; set; } = string.Empty;
}

public class CustomCredentialTypeViewModel
{
    [JsonProperty("credentialTypeName")]
    public string CredentialTypeName { get; set; } = string.Empty;

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("active")]
    public bool Active { get; set; }
}

public class CustomPaymentApiResult
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("data")]
    public List<CustomPaymentClientProviderConfigurationViewModel> Data { get; set; } = new();
}