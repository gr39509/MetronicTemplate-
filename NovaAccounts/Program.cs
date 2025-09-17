using Microsoft.AspNetCore.Components.Authorization;
using NovaAccounts.Components;
using NovaAccounts.Services.AuthService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
//builder.Services.AddHttpClient();

builder.Services.AddHttpClient("Auth", client =>
{
    client.BaseAddress = new Uri("http://185.132.37.188:8070");
});

builder.Services.AddSingleton<AuthenticationService>();
builder.Services.AddSingleton<TokenProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();