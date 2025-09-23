using System.Text.Json.Serialization;

namespace NovaAccounts.Models.Charts.DonutChart;

public class DonutChartData
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
        
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}