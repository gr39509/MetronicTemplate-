namespace NovaAccounts.Components.DataFields;

public class DataTableOptions
{
    private string _tableCssClass = string.Empty;

    public int PageLength { get; set; } = 10;
    public int[] LengthMenu { get; set; } = { 5, 10, 25, 50, -1 };
    public string[] LengthMenuLabels { get; set; } = { "5", "10", "25", "50", "All" };
    public bool ShowSearch { get; set; } = true;
    public bool ShowPaging { get; set; } = true;
    public bool ShowInfo { get; set; } = true;
    public bool ShowLengthMenu { get; set; } = true;
    public string SearchPlaceholder { get; set; } = "Search...";
    public string TableId { get; set; } = $"datatable_{Guid.NewGuid():N}";
    
    public string TableCssClass
    {
        get 
        { 
            if (string.IsNullOrEmpty(_tableCssClass))
            {
                return $"table {(Striped ? "table-striped" : "")}".Trim();
            }
            return _tableCssClass;
        }
        set => _tableCssClass = value;
    }
    
    public string HeaderCssClass { get; set; } = "leadBold bg-blue-chambray font-white";
    public string RowCssClass { get; set; } = "lead font-blue-ebonyclay";
    public bool Responsive { get; set; } = true;
    public bool Striped { get; set; } = false;
    
    public Dictionary<string, object> AdditionalOptions { get; set; } = new();
}

// namespace NovaAccounts.Components.DataFields;
//
// public class DataTableOptions
// {
//     public int PageLength { get; set; } = 10;
//     public int[] LengthMenu { get; set; } = { 5, 10, 25, 50, -1 };
//     public string[] LengthMenuLabels { get; set; } = { "5", "10", "25", "50", "All" };
//     public bool ShowSearch { get; set; } = true;
//     public bool ShowPaging { get; set; } = true;
//     public bool ShowInfo { get; set; } = true;
//     public bool ShowLengthMenu { get; set; } = true;
//     public string SearchPlaceholder { get; set; } = "Search...";
//     public string TableId { get; set; } = $"datatable_{Guid.NewGuid():N}";
//     public string TableCssClass
//     {
//         get { return $"table {(Striped? "table-striped" : "" )}";}
//         set => TableCssClass = value;
//     }
//     public string HeaderCssClass { get; set; } = "leadBold bg-blue-chambray font-white";
//     public string RowCssClass { get; set; } = "lead font-blue-ebonyclay";
//     public bool Responsive { get; set; } = true;
//     public bool Striped { get; set; } = false;
//     
//     
//     public Dictionary<string, object> AdditionalOptions { get; set; } = new();
// }
