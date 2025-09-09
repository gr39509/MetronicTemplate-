namespace NovaAccounts.Components.UsageModes;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-25);
    public bool IsActive { get; set; } = true;
}