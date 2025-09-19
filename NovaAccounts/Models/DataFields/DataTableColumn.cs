namespace NovaAccounts.Components.DataFields;

public class DataTableColumn
{
    public string Header { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string DataType { get; set; } = "string"; // "string", "date", "currency", "number"
    public bool Sortable { get; set; } = true;
    public bool Searchable { get; set; } = true;
    public string CssClass { get; set; } = string.Empty;
    public Func<object, string>? CustomRenderer { get; set; }
    public int Width { get; set; } = 0; // 0 means auto width
}