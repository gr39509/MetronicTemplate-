namespace NovaAccounts.SharedModels.UserProfile;

public class FormField
{
    public string Name { get; set; }
    public string Label { get; set; }
    public FormFieldType FieldType { get; set; }
    public string Value { get; set; }
    public DateTime? DateValue { get; set; }
    public int? NumericValue { get; set; }
    public int? Day { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public List<SelectOption> Options { get; set; } = new();
    public string Placeholder { get; set; } = "";
    public bool BoolValue { get; set; }
    public bool IsRequired { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }
    public int Rows { get; set; } = 3;
}