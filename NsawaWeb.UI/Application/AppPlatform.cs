using Microsoft.AspNetCore.Components;

namespace NsawaWeb.Application;

/// <summary>
/// What the shared pages ask of the device they run on. The website uses <see cref="WebPlatform"/>;
/// the mobile app registers its own implementation with native share, save, in-app browser and app lock.
/// </summary>
public interface IAppPlatform
{
    /// <summary>True inside the mobile app. Pages show Share / Save buttons only then.</summary>
    bool IsNativeApp { get; }

    /// <summary>Base address for links people open outside the app, such as the donation page. Ends with "/".
    /// Null when the app wasn't configured with one; pages then don't offer a link at all rather than a broken one.</summary>
    string? PublicBaseUri { get; }

    /// <summary>Opens the payment partner's page.</summary>
    Task OpenPaymentAsync(string url);

    Task ShareLinkAsync(string title, string url);

    /// <summary>Opens the share sheet with a file, e.g. the QR code PNG or the donations CSV.</summary>
    Task ShareFileAsync(string title, byte[] content, string fileName, string contentType);

    /// <summary>Saves a PNG to the phone's photo gallery. Returns false when the user refused permission.</summary>
    Task<bool> SaveImageAsync(byte[] png, string fileName);

    /// <summary>True when the device has a fingerprint, face or screen lock the app can use.</summary>
    bool CanUseAppLock { get; }

    bool AppLockEnabled { get; }

    /// <summary>Turns the app lock on or off. Turning it on asks the user to unlock once first; returns the new state.</summary>
    Task<bool> SetAppLockAsync(bool enabled);
}

/// <summary>Browser behaviour: full-page navigation to payments, links on this site's own address, no native extras.</summary>
public sealed class WebPlatform(NavigationManager navigation) : IAppPlatform
{
    public bool IsNativeApp => false;
    public string? PublicBaseUri => navigation.BaseUri;

    public Task OpenPaymentAsync(string url)
    {
        navigation.NavigateTo(url, forceLoad: true);
        return Task.CompletedTask;
    }

    // Pages only offer these when IsNativeApp is true.
    public Task ShareLinkAsync(string title, string url) => throw new NotSupportedException();
    public Task ShareFileAsync(string title, byte[] content, string fileName, string contentType) => throw new NotSupportedException();
    public Task<bool> SaveImageAsync(byte[] png, string fileName) => throw new NotSupportedException();

    public bool CanUseAppLock => false;
    public bool AppLockEnabled => false;
    public Task<bool> SetAppLockAsync(bool enabled) => Task.FromResult(false);
}
