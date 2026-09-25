namespace NsawaWeb.Application;

/// <summary>Support contact details shown on the landing page. Each one is hidden until it is set.</summary>
public sealed class ContactOptions
{
    public const string SectionName = "Contact";

    public string? Phone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }

    public bool Any => !string.IsNullOrWhiteSpace(Phone) || !string.IsNullOrWhiteSpace(WhatsApp) || !string.IsNullOrWhiteSpace(Email);
}
