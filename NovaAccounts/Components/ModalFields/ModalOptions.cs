using Microsoft.AspNetCore.Components;

namespace NovaAccounts.Components.ModalFields;

public class ModalOptions
{
    public string Id { get; set; } = $"modal_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    public string Title { get; set; } = "Modal";
    public ModalSize Size { get; set; } = ModalSize.Default;
    public bool ShowHeader { get; set; } = true;
    public bool ShowFooter { get; set; } = true;
    public bool ShowCloseButton { get; set; } = true;
    public bool Backdrop { get; set; } = true; // true, false, or 'static'
    public bool Keyboard { get; set; } = true; // Allow ESC to close
    public bool Focus { get; set; } = true;
    public bool Centered { get; set; } = false;
    public bool Scrollable { get; set; } = false;
    public string HeaderCssClass { get; set; } = string.Empty;
    public string BodyCssClass { get; set; } = string.Empty;
    public string FooterCssClass { get; set; } = string.Empty;
    public string TitleCssClass { get; set; } = "modal-title fs-5";
    public List<ModalButton> FooterButtons { get; set; } = new();
    public EventCallback OnShow { get; set; }
    public EventCallback OnHide { get; set; }
    public EventCallback OnShown { get; set; }
    public EventCallback OnHidden { get; set; }
}