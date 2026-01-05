namespace NovaAccounts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public class MenuBase : ComponentBase, IDisposable
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    protected virtual void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    protected string GetCurrentController()
    {
        var uri = new Uri(NavigationManager.Uri);
        var segments = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 0 ? segments[0] : "";
    }

    protected string GetCurrentAction()
    {
        var uri = new Uri(NavigationManager.Uri);
        var segments = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 ? segments[1] : "";
    }

    protected bool IsControllerActive(List<string> controllers)
    {
        var currentController = GetCurrentController();
        return controllers.Any(c => c.Equals(currentController, StringComparison.InvariantCultureIgnoreCase));
    }

    protected bool IsCurrentPage(string path)
    {
        var currentUri = new Uri(NavigationManager.Uri);
        var currentPath = currentUri.LocalPath.TrimEnd('/');
        var comparePath = path.TrimEnd('/');
        
        // Handle root path
        if (string.IsNullOrEmpty(currentPath) && (comparePath == "/" || string.IsNullOrEmpty(comparePath)))
            return true;
            
        return currentPath.Equals(comparePath, StringComparison.InvariantCultureIgnoreCase);
    }

    protected bool IsCurrentPage(string controller, string action)
    {
        var currentController = GetCurrentController();
        var currentAction = GetCurrentAction();
        
        return currentController.Equals(controller, StringComparison.InvariantCultureIgnoreCase) &&
               currentAction.Equals(action, StringComparison.InvariantCultureIgnoreCase);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}