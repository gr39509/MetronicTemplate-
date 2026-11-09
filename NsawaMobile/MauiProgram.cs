using System.Reflection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NsawaMobile.Native;
using NsawaWeb.Application;
using NsawaWeb.Services.Services;

namespace NsawaMobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>();

		using (var settings = Assembly.GetExecutingAssembly().GetManifestResourceStream("appsettings.json")!)
		{
			builder.Configuration.AddJsonStream(settings);
		}

		builder.Services.AddMauiBlazorWebView();

		builder.Services.AddOptions<ApiOptions>()
			.Bind(builder.Configuration.GetSection(ApiOptions.SectionName))
			.Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _), "Api:BaseUrl must be an absolute URL.");
		builder.Services.Configure<ContactOptions>(builder.Configuration.GetSection(ContactOptions.SectionName));

		// Same wiring as the web host, minus the shared socket pool: the app has one webview, so one scope.
		builder.Services.AddScoped(sp =>
		{
			var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
			var handler = new AuthHeaderHandler(sp.GetRequiredService<AuthService>()) { InnerHandler = new HttpClientHandler() };
			return new ApiClient(new HttpClient(handler) { BaseAddress = new Uri(options.BaseUrl) }, options.BaseUrl);
		});
		builder.Services.AddHttpClient(ImageProxyClient.Name, (sp, http) =>
		{
			http.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ApiOptions>>().Value.BaseUrl);
			http.Timeout = TimeSpan.FromSeconds(20);
		});

		// Links people open outside the app (donation page, QR) must point at the public website, not the app's own address.
		var publicSite = Uri.TryCreate(builder.Configuration["PublicSiteUrl"], UriKind.Absolute, out var site) ? site.AbsoluteUri.TrimEnd('/') + "/" : null;
		builder.Services.AddSingleton<IAppPlatform>(new MauiPlatform(publicSite));
		builder.Services.AddSingleton<ITokenStorage, SecureTokenStorage>();
		builder.Services.AddScoped<AuthService>();
		builder.Services.AddScoped<CustomAuthStateProvider>();
		builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
		builder.Services.AddAuthorizationCore();
		builder.Services.AddCascadingAuthenticationState();
		builder.Services.AddNsawaApplication();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
