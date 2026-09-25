using System.Text;
using NsawaWeb.Application;
using NsawaWeb.Services.Services;

namespace NsawaWeb.Features.Donations;

/// <summary>Display rules for donation rows, shared by the table, the detail dialog and the CSV export.</summary>
public static class DonationDisplay
{
    public static string DonorName(DonationViewModel d) =>
        string.IsNullOrWhiteSpace(d.DonorName) ? "Anonymous" : d.DonorName.Trim();

    public static string Source(DonationViewModel d) =>
        string.IsNullOrWhiteSpace(d.Source) ? "Direct" : d.Source.Trim();

    /// <summary>Badge colour for a donation's payment type name, matched against the lookup list.</summary>
    public static string? BadgeKind(string? paymentTypeName, IReadOnlyList<PaymentTypeViewModel> paymentTypes)
    {
        if (string.IsNullOrWhiteSpace(paymentTypeName)) return null;
        var type = paymentTypes.FirstOrDefault(t => string.Equals(t.Name, paymentTypeName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (type is null) return null;
        if (type.IsMoMo) return "brand";
        if (type.IsCash) return "gold";
        if (type.IsCheque) return "info";
        return null;
    }

    public static string ToCsv(IEnumerable<DonationViewModel> donations)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date,Donor,Phone,Payment type,Method,Provider,Transaction ID,Reference,Source,Group,Amount (GHS),Message");
        foreach (var d in donations)
        {
            sb.AppendJoin(',',
                Cell(d.DateDonated.ToString("yyyy-MM-dd HH:mm")),
                Cell(DonorName(d)),
                Cell(d.DonorPhoneNumber),
                Cell(d.PaymentType),
                Cell(d.PaymentMethod),
                Cell(d.PaymentProvider),
                Cell(d.TransactionId),
                Cell(d.PaymentReference),
                Cell(Source(d)),
                Cell(d.Group?.GroupName),
                d.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                Cell(d.Message));
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string Cell(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        // Stop spreadsheet apps treating donor-entered text as a formula.
        if (value[0] is '=' or '+' or '-' or '@' or '\t' or '\r') value = "'" + value;
        return value.IndexOfAny([',', '"', '\n', '\r']) >= 0 ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }
}
