namespace NsawaMobile;

public partial class App : Application
{
	// Coming back sooner than this (e.g. after sharing to WhatsApp or paying) doesn't ask to unlock again.
	private static readonly TimeSpan LockAfter = TimeSpan.FromMinutes(1);

	public App()
	{
		InitializeComponent();
		// The pages are designed light-only; this keeps the native bars and lock screen light in system dark mode too.
		UserAppTheme = AppTheme.Light;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var page = new MainPage();
		var window = new Window(page) { Title = "Nova Nsawa" };
		DateTime? leftAt = null;
		window.Stopped += (_, _) => leftAt = DateTime.UtcNow;
		window.Resumed += async (_, _) =>
		{
			if (leftAt is { } t && DateTime.UtcNow - t >= LockAfter) await page.LockIfNeededAsync();
		};
		return window;
	}
}
