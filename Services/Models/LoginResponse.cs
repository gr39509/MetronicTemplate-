using System.Text.Json.Serialization;

namespace NovaAccounts.Services.Models;


public class LoginResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("instance")]
    public string Instance { get; set; } = string.Empty;

    [JsonPropertyName("payload")]
    public LoginPayload? Payload { get; set; }
}