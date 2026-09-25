using System.Globalization;
using System.Text.RegularExpressions;

namespace NsawaWeb.Application;

/// <summary>Display and input helpers shared across pages.</summary>
public static partial class Format
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-GB");

    public static string Money(double amount) => amount.ToString("N2", Culture);
    public static string Cedis(double amount) => "GHS " + Money(amount);

    public static string Date(DateTimeOffset value) => value.ToString("d MMM yyyy", Culture);
    public static string ShortDate(DateTimeOffset value) => value.ToString("d MMM", Culture);
    public static string DateTime(DateTimeOffset value) => value.ToString("d MMM yyyy, HH:mm", Culture);

    public static string DateRange(DateTimeOffset start, DateTimeOffset end)
    {
        if (start.Date == end.Date) return Date(start);
        if (start.Year == end.Year && start.Month == end.Month) return $"{start.Day}–{end.ToString("d MMM yyyy", Culture)}";
        if (start.Year == end.Year) return $"{ShortDate(start)} – {Date(end)}";
        return $"{Date(start)} – {Date(end)}";
    }

    public static string Initials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 1
            ? parts[0][..Math.Min(2, parts[0].Length)].ToUpperInvariant()
            : $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
    }

    /// <summary>Friendly label for the API's network ids.</summary>
    public static string Network(string? id) => id?.ToUpperInvariant() switch
    {
        "MTN" => "MTN MoMo",
        "VODAFONE" or "TELECEL" => "Telecel Cash",
        "AIRTEL" or "AIRTELTIGO" or "AT" => "AT Money",
        null or "" => "—",
        _ => id
    };

    public static string DigitsOnly(string? value) => value is null ? string.Empty : NonDigits().Replace(value, string.Empty);

    /// <summary>True for a Ghana mobile number written locally, e.g. 0241234567.</summary>
    public static bool IsLocalPhone(string? value) => LocalPhone().IsMatch(value ?? string.Empty);

    /// <summary>0241234567 → 233241234567.</summary>
    public static string ToInternational(string local) => "233" + DigitsOnly(local).TrimStart('0');

    public static string BannerUrl(string? banner) =>
        string.IsNullOrWhiteSpace(banner) ? string.Empty : $"/api/imageproxy/getimage/{Uri.EscapeDataString(banner)}";

    public static string EventTitle(string? title) =>
        string.IsNullOrWhiteSpace(title) ? "Untitled event" : Culture.TextInfo.ToTitleCase(title.Trim().ToLower(Culture));

    [GeneratedRegex("[^0-9]")]
    private static partial Regex NonDigits();

    [GeneratedRegex("^0[235][0-9]{8}$")]
    private static partial Regex LocalPhone();
}
