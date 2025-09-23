using System.Text.Json.Serialization;

namespace NovaAccounts.Models.Charts.LineChart;

public class LineChartData
{
    [JsonPropertyName("year")]
    public string Year { get; set; } = string.Empty;
        
    [JsonPropertyName("a")]
    public decimal A { get; set; }
        
    [JsonPropertyName("b")]
    public decimal B { get; set; }
}