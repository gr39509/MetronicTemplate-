namespace NsawaWeb.Features.Public;

/// <summary>Small display helpers for the donor pages.</summary>
internal static class DonateText
{
    /// <summary>0241234567 → "024 123 4567", easier to check at a glance.</summary>
    public static string Phone(string? local)
    {
        var digits = Application.Format.DigitsOnly(local);
        return digits.Length == 10 ? $"{digits[..3]} {digits[3..6]} {digits[6..]}" : digits;
    }
}
