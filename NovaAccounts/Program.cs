using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using NovaAccounts.Components;
using NovaAccounts.Components.APIConsummation.Debug;
using NovaAccounts.SharedModels.ApiService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true; // Enable for debugging
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

//builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();

// builder.Services.AddScoped(sp => new HttpClient
// {
//     BaseAddress = new Uri("http://185.132.37.188:5672")
// });
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
//builder.Services.AddScoped<AuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();