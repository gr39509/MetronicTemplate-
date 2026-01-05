using Microsoft.AspNetCore.Components.Authorization;
using NovaAccounts.Components;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
//builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddHttpClient<NovaAccounts.Shared.ApiClient>((serviceProvider, client) =>
{
 
    client.BaseAddress = new Uri("http://20.218.238.86");

    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddScoped<NovaAccounts.Shared.ApiClient>(serviceProvider =>
{
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient(nameof(NovaAccounts.Shared.ApiClient));
    
    return new NovaAccounts.Shared.ApiClient(httpClient, string.Empty);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();