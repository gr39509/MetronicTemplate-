namespace NovaAccounts.SharedModels.Charts.DonutChart;

public class DonutChartOptions
{
    public string ElementId { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new();
    public List<DonutChartData> Data { get; set; } = new();
}