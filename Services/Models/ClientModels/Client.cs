namespace NovaAccounts.Services.Models.ClientModels;

public class Client
{
    public int Id { get; set; }
    public string ClientID { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? AccountName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
}