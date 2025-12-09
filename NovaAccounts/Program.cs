using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NovaAccounts.Components;
using NovaAccounts.Components.APIConsummation.Debug;
using NovaAccounts.Services;
using NovaAccounts.Services.NovaAccounts.Services;
using NovaAccounts.SharedModels.ApiService;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(2);
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    });

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

builder.Services.AddSingleton<CircuitHandler, CustomCircuitHandler>();


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddScoped<ApiClient>(provider =>
{
    var authStateProvider = provider.GetRequiredService<AuthenticationStateProvider>();
    
    var authenticatedHttpClient = new HttpClient(new AuthHeaderHandler(authStateProvider)
    {
        InnerHandler = new HttpClientHandler()
    })
    {
        BaseAddress = new Uri("http://185.132.37.188:5672")
    };
    
    return new ApiClient(authenticatedHttpClient, "http://185.132.37.188:5672");
});



builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddCascadingAuthenticationState();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();