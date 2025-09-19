namespace NovaAccounts.Models.ProgressIndicatorModel;

public class ProgressIndicator
{
    public string Value { get; set; } = "0";
    public string DisplayValue { get; set; } = "";
    public string Label { get; set; } = "STAT LABEL";
    public string IconClass { get; set; } = "icon-bar-chart";
    public int ProgressPercentage { get; set; } = 0;
    public string ProgressBarClass { get; set; } = "progress-bar-success green-sharp";
    public string ValueColorClass { get; set; } = "font-green-sharp";
    public string ProgressLabel { get; set; } = "progress";
    public string ColClass { get; set; } = "col-lg-3 col-md-3 col-sm-6 col-xs-12";
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
}