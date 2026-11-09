using Foundation;
using LocalAuthentication;
using Photos;
using UIKit;

namespace NsawaMobile.Native;

public sealed partial class MauiPlatform
{
    // Face ID / Touch ID, falling back to the device passcode.
    private static partial bool CanAuthenticate() =>
        new LAContext().CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out _);

    private static partial async Task<bool> AuthenticatePlatformAsync()
    {
        var (success, _) = await new LAContext().EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthentication, "Unlock Nova Nsawa");
        return success;
    }

    private static partial async Task<bool> SaveToGalleryAsync(byte[] png, string fileName)
    {
        if (await Permissions.RequestAsync<Permissions.PhotosAddOnly>() != PermissionStatus.Granted) return false;
        var image = UIImage.LoadFromData(NSData.FromArray(png)) ?? throw new IOException("Not a valid image.");
        var done = new TaskCompletionSource<bool>();
        PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
            () => PHAssetChangeRequest.FromImage(image),
            (ok, error) => { if (ok) done.TrySetResult(true); else done.TrySetException(new IOException(error?.LocalizedDescription)); });
        return await done.Task;
    }
}
