// using Microsoft.AspNetCore.Components.Authorization;
// using NovaAccounts;
// using NovaAccounts.Components;
// using NovaAccounts.Services;
// using NovaAccounts.Services.AuthServices;
// using NovaAccounts.Services.ClientsServices;
// using NovaAccounts.SharedModels;
// using NovaAccounts.SharedModels.ApiService;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// builder.Services.AddRazorComponents()
//     .AddInteractiveServerComponents();
//
// // Add Blazor services
// builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
//
// // HTTP Client for general use
// builder.Services.AddScoped(sp => new HttpClient 
// { 
//     BaseAddress = new Uri("http://185.132.37.188:8070") 
// });
//
// // Custom authentication handler
// builder.Services.AddScoped<AuthHeaderHandler>();
//
// // ApiClient with authentication
// builder.Services.AddScoped<ApiClient>(provider =>
// {
//     var httpClient = provider.GetRequiredService<HttpClient>();
//     var authStateProvider = provider.GetRequiredService<AuthenticationStateProvider>();
//     
//     var authenticatedHttpClient = new HttpClient(new AuthHeaderHandler(authStateProvider)
//     {
//         InnerHandler = new HttpClientHandler()
//     })
//     {
//         BaseAddress = new Uri("http://185.132.37.188:8070")
//     };
//     
//     return new ApiClient(authenticatedHttpClient, "http://185.132.37.188:8070");
// });
//
// // Application services
// builder.Services.AddScoped<AuthService>();
// builder.Services.AddScoped<ClientService>();
// builder.Services.AddScoped<ClientGeneratorService>();
//
// // Blazor Authentication (NO ASP.NET Core Authentication)
// builder.Services.AddCascadingAuthenticationState();
// builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
//
// // Authorization for component-level [Authorize] attributes only
//
// builder.Services.AddAuthorization();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }
//
// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseAntiforgery();
// app.UseAuthentication();
// app.UseAuthorization();
//
// // DO NOT add UseAuthentication() or UseAuthorization() middleware
//
// app.MapRazorComponents<App>()
//     .AddInteractiveServerRenderMode();
//
// app.Run();



using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NovaAccounts;
using NovaAccounts.Components;
using NovaAccounts.Services;
using NovaAccounts.Services.AuthServices;

using NovaAccounts.SharedModels;
using NovaAccounts.SharedModels.ApiService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Protected Storage BEFORE creating ApiClient
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<ProtectedLocalStorage>();

// Configure HttpClient with base address
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://185.132.37.188:8070")
});

builder.Services.AddScoped<ClientGeneratorService>();

// Register ApiClient with auth header handler
builder.Services.AddScoped<ApiClient>(provider =>
{
    var authStateProvider = provider.GetRequiredService<AuthenticationStateProvider>();
    
    var authenticatedHttpClient = new HttpClient(new AuthHeaderHandler(authStateProvider)
    {
        InnerHandler = new HttpClientHandler()
    })
    {
        BaseAddress = new Uri("http://185.132.37.188:8070")
    };
    
    return new ApiClient(authenticatedHttpClient, "http://185.132.37.188:8070");
});

builder.Services.AddScoped<AuthHeaderHandler>();

// Add Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "novaaccounts.auth";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(72);
    });

builder.Services.AddAuthorization();

// Register authentication services - CustomAuthStateProvider MUST come after ProtectedStorage registration
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();

// builder.Services.AddScoped<AuthService>();
// builder.Services.AddScoped<ClientService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();