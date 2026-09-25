using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using NsawaWeb.Application;
using NsawaWeb.Components;
using NsawaWeb.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection(ApiOptions.SectionName))
    .Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _), "Api:BaseUrl must be an absolute URL.")
    .ValidateOnStart();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// One pooled socket handler for the whole app; each circuit wraps it with its own auth handler.
builder.Services.AddSingleton(_ => new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) });
builder.Services.AddScoped(sp =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    var handler = new AuthHeaderHandler(sp.GetRequiredService<AuthService>())
    {
        InnerHandler = sp.GetRequiredService<SocketsHttpHandler>()
    };
    var http = new HttpClient(handler, disposeHandler: false) { BaseAddress = new Uri(options.BaseUrl) };
    return new ApiClient(http, options.BaseUrl);
});

// Anonymous client for the banner image proxy (plain HTTP requests have no browser token).
builder.Services.AddHttpClient(ImageProxyClient.Name, (sp, http) =>
{
    http.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiOptions>>().Value.BaseUrl);
    http.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddNsawaApplication();
builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
