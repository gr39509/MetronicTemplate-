namespace NovaAccounts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public class MenuBase : ComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;

    protected string GetCurrentController()
    {
        var uri = new Uri(NavigationManager.Uri);
        var segments = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 0 ? segments[0] : "";
    }

  

    protected bool IsControllerActive(List<string> controllers)
    {
        var currentController = GetCurrentController();
        return controllers.Any(c => c.Equals(currentController, StringComparison.InvariantCultureIgnoreCase));
    }

    protected bool IsCurrentPage(string path)
    {
        
        
        var uri = new Uri(NavigationManager.Uri);
        return uri.LocalPath.Equals(path, StringComparison.InvariantCultureIgnoreCase);;
    }
}