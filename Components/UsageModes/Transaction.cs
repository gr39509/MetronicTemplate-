namespace NovaAccounts.Components.UsageModes;

public class Transaction
{
    public DateTime TransDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string StatusBadge => Status switch
    {
        "Completed" => "<span class='badge badge-success'>Completed</span>",
        "Pending" => "<span class='badge badge-warning'>Pending</span>",
        "Failed" => "<span class='badge badge-danger'>Failed</span>",
        "Processing" => "<span class='badge badge-info'>Processing</span>",
        _ => $"<span class='badge badge-secondary'>{Status}</span>"
    };
}