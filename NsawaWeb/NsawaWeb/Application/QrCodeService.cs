using QRCoder;
using SkiaSharp;

namespace NsawaWeb.Application;

/// <summary>Builds the printable donation QR code shown at events.</summary>
public sealed class QrCodeService
{
    private static readonly SKColor Emerald = new(0x0E, 0x6B, 0x53);

    /// <returns>A PNG as a data URI, ready for an img src or a download link.</returns>
    public string CreateDonationQrDataUri(string donateUrl)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(donateUrl, QRCodeGenerator.ECCLevel.H);
        using var png = new PngByteQRCode(data);
        var raw = png.GetGraphic(20);

        using var source = SKBitmap.Decode(raw);
        using var surface = SKSurface.Create(new SKImageInfo(source.Width, source.Height));
        var canvas = surface.Canvas;
        canvas.DrawBitmap(source, 0, 0);

        // A badge in the centre; error correction level H leaves room for it.
        float cx = source.Width / 2f, cy = source.Height / 2f, radius = source.Width / 10f;
        using var ring = new SKPaint { Color = SKColors.White, IsAntialias = true };
        using var fill = new SKPaint { Color = Emerald, IsAntialias = true };
        canvas.DrawCircle(cx, cy, radius + 10, ring);
        canvas.DrawCircle(cx, cy, radius, fill);

        using var font = new SKFont(SKTypeface.FromFamilyName(null, SKFontStyle.Bold), radius * 0.36f);
        using var text = new SKPaint { Color = SKColors.White, IsAntialias = true };
        canvas.DrawText("NSAWA", cx, cy - font.Size * 0.1f, SKTextAlign.Center, font, text);
        canvas.DrawText("DONATE", cx, cy + font.Size * 1.0f, SKTextAlign.Center, font, text);

        using var image = surface.Snapshot();
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return "data:image/png;base64," + Convert.ToBase64String(encoded.ToArray());
    }
}
