namespace NovaAccounts.SharedModels.UserProfile;

public class ActionButton
{
    public string Label { get; set; }
    public ButtonType ButtonType { get; set; }
    public string ColorClass { get; set; } = "primary";
    public string Url { get; set; }
    public Action OnClick { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }
}