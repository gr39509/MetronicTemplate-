using System.Text.Json.Serialization;

namespace NovaAccounts.Services.Models;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}