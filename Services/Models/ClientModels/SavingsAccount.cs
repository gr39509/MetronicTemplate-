namespace NovaAccounts.Services.Models.ClientModels;

public class SavingsAccount
{
    public int Id { get; set; }
    public int SavingsProductID { get; set; }
    public string SavingsProductCode { get; set; } = string.Empty;
    public string SavingsProduct { get; set; } = string.Empty;
    public string ClientNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal AccountBalance { get; set; }
    public DateTime AccountOpeningDate { get; set; }
    public string RelationshipOfficer { get; set; } = string.Empty;
}
