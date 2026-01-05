namespace NovaAccounts.SharedModels.QuickNav;


public class NavItem
{
    public string Text { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public Action? OnClick { get; set; } 
    public bool PreventNavigation { get; set; } 
}