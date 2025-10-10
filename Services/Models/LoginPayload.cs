using System.Text.Json.Serialization;

namespace NovaAccounts.Services.Models;


public class LoginPayload
{
    [JsonPropertyName("requiresTwoFactor")]
    public bool RequiresTwoFactor { get; set; }

    [JsonPropertyName("authResponse")]
    public AuthResponse? AuthResponse { get; set; }
}