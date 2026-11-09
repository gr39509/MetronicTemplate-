using Android.Content;
using Android.Hardware.Biometrics;
using Android.Media;
using Android.OS;
using Android.Provider;
using Java.Lang;
using AEnvironment = Android.OS.Environment;

namespace NsawaMobile.Native;

public sealed partial class MauiPlatform
{
    // Fingerprint/face, or the phone's PIN/pattern as a fallback. Needs Android 11 (API 30) for this combination.
    private const int Authenticators = (int)(BiometricManagerAuthenticators.BiometricWeak | BiometricManagerAuthenticators.DeviceCredential);

    private static partial bool CanAuthenticate()
    {
        try
        {
            return OperatingSystem.IsAndroidVersionAtLeast(30)
                   && Platform.AppContext.GetSystemService(Context.BiometricService) is BiometricManager manager
                   && manager.CanAuthenticate(Authenticators) == BiometricCode.Success;
        }
        catch (Java.Lang.Exception)
        {
            // The account menu asks this while rendering; a device quirk must hide the option, not break the menu.
            return false;
        }
    }

    private static partial Task<bool> AuthenticatePlatformAsync()
    {
        if (!CanAuthenticate() || Platform.CurrentActivity is not { } activity) return Task.FromResult(false);

        var done = new TaskCompletionSource<bool>();
        var prompt = new BiometricPrompt.Builder(activity)
            .SetTitle("Unlock Nova Nsawa")
            .SetAllowedAuthenticators(Authenticators)
            .Build();
        prompt.Authenticate(new CancellationSignal(), activity.MainExecutor!, new Callback(done));
        return done.Task;
    }

    private sealed class Callback(TaskCompletionSource<bool> done) : BiometricPrompt.AuthenticationCallback
    {
        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult? result) => done.TrySetResult(true);

        // Cancelled, too many attempts, etc. A single wrong finger keeps the prompt open, so OnAuthenticationFailed is ignored.
        public override void OnAuthenticationError(BiometricErrorCode errorCode, ICharSequence? errString) => done.TrySetResult(false);
    }

    private static partial async Task<bool> SaveToGalleryAsync(byte[] png, string fileName)
    {
        var context = Platform.AppContext;
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            // Android 10+: MediaStore needs no permission for the app's own pictures.
            var values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            values.Put(MediaStore.IMediaColumns.MimeType, "image/png");
            values.Put(MediaStore.IMediaColumns.RelativePath, $"{AEnvironment.DirectoryPictures}/Nova Nsawa");
            var uri = context.ContentResolver!.Insert(MediaStore.Images.Media.ExternalContentUri!, values)
                      ?? throw new IOException("The gallery refused the image.");
            await using var stream = context.ContentResolver.OpenOutputStream(uri) ?? throw new IOException("Could not open the gallery file.");
            await stream.WriteAsync(png);
            return true;
        }

        if (await Permissions.RequestAsync<Permissions.StorageWrite>() != PermissionStatus.Granted) return false;
        var folder = Path.Combine(AEnvironment.GetExternalStoragePublicDirectory(AEnvironment.DirectoryPictures)!.AbsolutePath, "Nova Nsawa");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);
        await File.WriteAllBytesAsync(path, png);
        MediaScannerConnection.ScanFile(context, [path], ["image/png"], null);
        return true;
    }
}
