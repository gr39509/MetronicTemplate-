using NsawaMobile.Native;
using NsawaWeb.Application;

namespace NsawaMobile;

public partial class MainPage : ContentPage
{
	// Same path the web host's ImageProxyController serves, so the shared pages need no changes.
	private const string BannerPath = "/api/imageproxy/getimage/";

	public MainPage()
	{
		InitializeComponent();
		blazorWebView.WebResourceRequested += OnWebResourceRequested;
		Connectivity.Current.ConnectivityChanged += (_, e) => ShowOffline(e.NetworkAccess);
		ShowOffline(Connectivity.Current.NetworkAccess);
#if ANDROID
		blazorWebView.HandlerChanged += (_, _) =>
		{
			// Registered after BlazorWebView's own handling, so Android asks this one first and stops there.
			// Without it one press both went back a page and sent the app to the background.
			if (blazorWebView.Handler?.PlatformView is not Android.Webkit.WebView web || web.Context is not AndroidX.Activity.ComponentActivity activity) return;
			var back = new BackCallback(web, activity, () => lockScreen.IsVisible);
			activity.OnBackPressedDispatcher.AddCallback(activity, back);
			// The back *key* (3-button navigation) also reaches the webview, which steps back by itself; take it here too.
			web.SetOnKeyListener(back);
		};
#endif
	}

#if ANDROID
	/// <summary>One back = one page back; on the first page, the app goes to the background like other Android apps.</summary>
	private sealed class BackCallback(Android.Webkit.WebView web, Android.App.Activity activity, Func<bool> isLocked)
		: AndroidX.Activity.OnBackPressedCallback(true), Android.Views.View.IOnKeyListener
	{
		private long _handledAt;

		public override void HandleOnBackPressed()
		{
			_handledAt = Android.OS.SystemClock.UptimeMillis();
			// While locked, back leaves the app; it must not move the hidden page behind the lock screen.
			if (!isLocked() && web.CanGoBack()) web.GoBack();
			else activity.MoveTaskToBack(true);
		}

		// Newer Android routes the back key through HandleOnBackPressed on key-down; older Android only sends the key.
		// Swallow the key either way (the webview would step back by itself too) and act only if this press wasn't handled yet.
		public bool OnKey(Android.Views.View? v, Android.Views.Keycode keyCode, Android.Views.KeyEvent? e)
		{
			if (keyCode != Android.Views.Keycode.Back || e is null) return false;
			if (e.Action == Android.Views.KeyEventActions.Up && _handledAt < e.DownTime) HandleOnBackPressed();
			return true;
		}
	}
#endif

	private bool _shown;

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (_shown) return;
		_shown = true;
		await LockIfNeededAsync();
	}

	private void ShowOffline(NetworkAccess access) =>
		MainThread.BeginInvokeOnMainThread(() => offlineBar.IsVisible = access != NetworkAccess.Internet);

	// ---------- App lock ----------

	/// <summary>Covers the app and asks to unlock, when the lock is on and someone is signed in.</summary>
	public async Task LockIfNeededAsync()
	{
		if (lockScreen.IsVisible || !await MauiPlatform.ShouldLockAsync()) return;
		lockScreen.IsVisible = true;
		await UnlockAsync();
	}

	private async void OnUnlockClicked(object? sender, EventArgs e) => await UnlockAsync();

	private async Task UnlockAsync()
	{
		if (await MauiPlatform.AuthenticateAsync()) lockScreen.IsVisible = false;
	}

	// ---------- Pull to refresh ----------

	private void OnRefreshing(object? sender, EventArgs e)
	{
		// Reloading restarts the page at the same address; the sign-in survives in secure storage.
#if ANDROID
		(blazorWebView.Handler?.PlatformView as Android.Webkit.WebView)?.Reload();
#elif IOS
		(blazorWebView.Handler?.PlatformView as WebKit.WKWebView)?.Reload();
#endif
		refreshView.IsRefreshing = false;
	}

	// ---------- Event banners ----------

	/// <summary>
	/// Serves event banners from native code: the app downloads them from the API with HttpClient
	/// and hands the bytes to the webview, so the webview never fetches from the HTTP-only API itself.
	/// </summary>
	private void OnWebResourceRequested(object? sender, WebViewWebResourceRequestedEventArgs e)
	{
		if (!e.Uri.AbsolutePath.StartsWith(BannerPath, StringComparison.OrdinalIgnoreCase)) return;

		var fileName = Uri.UnescapeDataString(e.Uri.AbsolutePath[BannerPath.Length..]);
		e.Handled = true;
		if (!IsSafeFileName(fileName))
		{
			e.SetResponse(404, "Not Found");
			return;
		}

		var headers = new Dictionary<string, string> { ["Content-Type"] = "image/jpeg", ["Cache-Control"] = "max-age=86400" };
		e.SetResponse(200, "OK", headers, DownloadAsync(fileName));
	}

	private static async Task<Stream?> DownloadAsync(string fileName)
	{
		try
		{
			var http = IPlatformApplication.Current!.Services.GetRequiredService<IHttpClientFactory>().CreateClient(ImageProxyClient.Name);
			var bytes = await http.GetByteArrayAsync($"api/files/{Uri.EscapeDataString(fileName)}");
			return new MemoryStream(bytes);
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
		{
			// The browser shows a broken image, same as the web proxy's 404.
			return Stream.Null;
		}
	}

	// Mirrors ImageProxyController.IsSafeFileName: the name comes from page content, so keep it to a single file.
	private static bool IsSafeFileName(string fileName) =>
		fileName.Length is > 0 and <= 255
		&& !fileName.Contains("..", StringComparison.Ordinal)
		&& fileName.IndexOfAny(['/', '\\', ':']) < 0
		&& !fileName.Any(char.IsControl);
}
