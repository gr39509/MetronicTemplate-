using Microsoft.AspNetCore.Components;

namespace NovaAccounts.Components.ModalFields;

public class ModalButton
{
    public string Text { get; set; } = string.Empty;
    public string CssClass { get; set; } = "btn btn-primary";
    public bool CloseModal { get; set; } = false;
    public EventCallback OnClick { get; set; }
    public bool Disabled { get; set; } = false;
    public string Icon { get; set; } = string.Empty; // For FontAwesome or other icon classes
}