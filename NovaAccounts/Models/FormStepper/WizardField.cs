namespace NovaAccounts.Models.FormStepper;

public class WizardField
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public WizardFieldType Type { get; set; }
    public object? Value { get; set; }
    public string? Placeholder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string ColClass { get; set; } = "col-md-6";
    public string LabelColClass { get; set; } = "col-md-4";
    public string InputColClass { get; set; } = "col-md-8";
    public List<WizardSelectOption> Options { get; set; } = new();
    public List<WizardCheckboxOption> CheckboxOptions { get; set; } = new();
    public Dictionary<string, object> Attributes { get; set; } = new();
    public string? ValidationMessage { get; set; }
}