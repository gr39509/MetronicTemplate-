using NsawaWeb.Application;
using NsawaWeb.Services.Services;

namespace NsawaMobile.Native;

/// <summary>Native versions of what the shared pages ask for: share sheet, photo gallery, in-app browser, app lock.</summary>
public sealed partial class MauiPlatform(string? publicBaseUri) : IAppPlatform
{
    private const string AppLockKey = "appLock";

    public bool IsNativeApp => true;
    public string? PublicBaseUri { get; } = publicBaseUri;

    // Chrome Custom Tabs / SFSafariViewController: the donor stays in the app and comes back with one tap.
    public Task OpenPaymentAsync(string url) => Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);

    public Task ShareLinkAsync(string title, string url) =>
        Share.Default.RequestAsync(new ShareTextRequest { Title = title, Subject = title, Text = title, Uri = url });

    public async Task ShareFileAsync(string title, byte[] content, string fileName, string contentType)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(path, content);
        await Share.Default.RequestAsync(new ShareFileRequest(title, new ShareFile(path, contentType)));
    }

    public Task<bool> SaveImageAsync(byte[] png, string fileName) => SaveToGalleryAsync(png, fileName);

    public bool CanUseAppLock => CanAuthenticate();
    public bool AppLockEnabled => Preferences.Default.Get(AppLockKey, false);

    public async Task<bool> SetAppLockAsync(bool enabled)
    {
        // Prove the user can unlock before turning it on, so nobody locks themselves out.
        if (enabled && !await AuthenticateAsync()) return false;
        Preferences.Default.Set(AppLockKey, enabled);
        return enabled;
    }

    /// <summary>True when the lock is on and someone is signed in, i.e. there is something to protect.</summary>
    public static async Task<bool> ShouldLockAsync() =>
        Preferences.Default.Get(AppLockKey, false) && await new SecureTokenStorage().GetAsync("authToken") is not null;

    /// <summary>Shows the fingerprint / face / screen-lock prompt. True when the user unlocked.</summary>
    public static Task<bool> AuthenticateAsync() => MainThread.InvokeOnMainThreadAsync(AuthenticatePlatformAsync);

    private static partial bool CanAuthenticate();
    private static partial Task<bool> AuthenticatePlatformAsync();
    private static partial Task<bool> SaveToGalleryAsync(byte[] png, string fileName);
}

/// <summary>Keeps the sign-in token in the Android Keystore / iOS Keychain instead of webview storage.</summary>
public sealed class SecureTokenStorage : ITokenStorage
{
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            var value = await SecureStorage.Default.GetAsync(key);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        catch (Exception)
        {
            // Keys can become unreadable after a reinstall or restore; treat it as signed out.
            SecureStorage.Default.Remove(key);
            return null;
        }
    }

    public Task SetAsync(string key, string value) => SecureStorage.Default.SetAsync(key, value);

    public Task RemoveAsync(string key)
    {
        SecureStorage.Default.Remove(key);
        return Task.CompletedTask;
    }
}
