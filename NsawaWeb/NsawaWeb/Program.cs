using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Authorization;
using NsawaWeb.Client.Pages;
using NsawaWeb.Components;
using NsawaWeb.Services;
using NsawaWeb.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<ApiClient>(provider =>
{
    var authService = provider.GetRequiredService<AuthService>();
    
    var authenticatedHttpClient = new HttpClient(new AuthHeaderHandler(authService)
    {
        InnerHandler = new HttpClientHandler()
    })
    {
        BaseAddress = new Uri(builder.Configuration.GetValue<string>("Api:BaseUrl"))
    };
   
    return new ApiClient(authenticatedHttpClient, builder.Configuration.GetValue<string>("Api:BaseUrl"));
});


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NsawaWeb.Client._Imports).Assembly);

app.Run();