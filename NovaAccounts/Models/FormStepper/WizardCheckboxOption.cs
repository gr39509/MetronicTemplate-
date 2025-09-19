namespace NovaAccounts.Models.FormStepper;

public class WizardCheckboxOption
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public bool IsChecked { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string ColClass { get; set; } = "col-xs-6";
}