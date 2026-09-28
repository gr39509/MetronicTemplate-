namespace NsawaWeb.Application;

/// <summary>Public contact details shown on the landing page. Anything left empty is hidden.</summary>
public sealed class ContactOptions
{
    public const string SectionName = "Contact";

    public string? Phone { get; set; }
    public List<string> OtherPhones { get; set; } = [];
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    /// <summary>Address lines, e.g. street, landmark, area, digital address.</summary>
    public List<string> Address { get; set; } = [];
    public string? TermsUrl { get; set; }
    public string? PrivacyUrl { get; set; }

    public bool Any => !string.IsNullOrWhiteSpace(Phone) || !string.IsNullOrWhiteSpace(WhatsApp)
                       || !string.IsNullOrWhiteSpace(Email) || Address.Count > 0;
}
