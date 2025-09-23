namespace NovaAccounts.Models.Charts.LineChart;

public class LineChartOptions
{
    public string ElementId { get; set; } = string.Empty;
    public bool ParseTime { get; set; } = false;
    public List<LineChartData> Data { get; set; } = new();
    public string XKey { get; set; } = "year";
    public List<string> YKeys { get; set; } = new();
    public List<string> Labels { get; set; } = new();
    public List<string> LineColors { get; set; } = new();
}