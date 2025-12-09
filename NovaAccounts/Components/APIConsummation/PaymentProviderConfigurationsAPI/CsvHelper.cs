namespace NovaAccounts.Components.APIConsummation.PaymentProviderConfigurationsAPI;

using System.Text;
public static class CsvHelper
{
    public static string ConvertToCsv<T>(IEnumerable<T> data, List<string> includedProperties = null)
    {
        var properties = GetExportProperties<T>(includedProperties);
        var csv = new StringBuilder();

        var header = string.Join(",", properties.Select(p => EscapeCsvValue(p.HeaderName)));
        csv.AppendLine(header);
        
        foreach (var item in data)
        {
            var row = string.Join(",", properties.Select(p => 
            {
                var value = p.Property.GetValue(item);
                var stringValue = FormatValue(value, p.Property);
                return EscapeCsvValue(stringValue);
            }));
            csv.AppendLine(row);
        }

        return csv.ToString();
    }

    private static List<ExportProperty> GetExportProperties<T>(List<string> includedProperties = null)
    {
        var allProperties = typeof(T).GetProperties();
        var exportProperties = new List<ExportProperty>();
        
        var columnMappings = new Dictionary<string, string>
        {
            ["Reference"] = "Transaction ID",
            ["ClientCode"] = "Client Code",
            ["Provider"] = "Provider",
            ["PhoneNumber"] = "Phone Number",
            ["Amount"] = "Amount",
            ["Currency"] = "Currency",
            ["InvoiceNo"] = "Invoice Number",
            ["Status"] = "Status",
            ["TransactionType"] = "Transaction Type",
            ["CreatedAt"] = "Created Date",
            ["ProcessedAt"] = "Processed Date"
        };

        // Define column order
        var columnOrder = new Dictionary<string, int>
        {
            ["Reference"] = 1,
            ["ClientCode"] = 2,
            ["Provider"] = 3,
            ["PhoneNumber"] = 4,
            ["Amount"] = 5,
            ["Currency"] = 6,
            ["InvoiceNo"] = 7,
            ["Status"] = 8,
            ["TransactionType"] = 9,
            ["CreatedAt"] = 10,
            ["ProcessedAt"] = 11
        };

        foreach (var prop in allProperties)
        {
            // Skip if property not in our mapping
            if (!columnMappings.ContainsKey(prop.Name))
                continue;

            if (includedProperties != null && !includedProperties.Contains(prop.Name))
                continue;

            exportProperties.Add(new ExportProperty
            {
                Property = prop,
                HeaderName = columnMappings[prop.Name],
                Order = columnOrder.ContainsKey(prop.Name) ? columnOrder[prop.Name] : 999
            });
        }

        return exportProperties.OrderBy(p => p.Order).ToList();
    }

    private static string FormatValue(object value, System.Reflection.PropertyInfo property)
    {
        if (value == null) return string.Empty;
        
        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss");
        }
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        if (property.PropertyType == typeof(DateTimeOffset?) && value is DateTimeOffset nullableDateTime)
        {
            return nullableDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("N2");
        }

        return value.ToString();
    }

    private static string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        return value;
    }

    private class ExportProperty
    {
        public System.Reflection.PropertyInfo Property { get; set; }
        public string HeaderName { get; set; }
        public int Order { get; set; }
    }
}