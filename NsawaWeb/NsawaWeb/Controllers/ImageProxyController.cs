using Microsoft.AspNetCore.Mvc;
using NsawaWeb.Application;

namespace NsawaWeb.Controllers;

/// <summary>
/// Serves event banners from the API's file store on this site's origin,
/// so pages never link to the API host directly.
/// </summary>
[ApiController]
[Route("api/imageproxy")]
public class ImageProxyController(IHttpClientFactory httpClientFactory, ILogger<ImageProxyController> logger) : ControllerBase
{
    [HttpGet("getimage/{fileName}")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetImage(string fileName, CancellationToken cancellationToken)
    {
        if (!IsSafeFileName(fileName))
        {
            return NotFound();
        }

        try
        {
            var client = httpClientFactory.CreateClient(ImageProxyClient.Name);
            using var response = await client.GetAsync($"api/files/{Uri.EscapeDataString(fileName)}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType is null || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/jpeg";
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return File(bytes, contentType);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(ex, "Could not fetch banner {FileName}", fileName);
            return NotFound();
        }
    }

    private static bool IsSafeFileName(string fileName) =>
        fileName.Length is > 0 and <= 255
        && !fileName.Contains("..", StringComparison.Ordinal)
        && fileName.IndexOfAny(['/', '\\', ':']) < 0
        && !fileName.Any(char.IsControl);
}
