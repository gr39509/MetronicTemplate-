namespace NovaAccounts.Services.Models;

public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public T? Payload { get; set; }
}