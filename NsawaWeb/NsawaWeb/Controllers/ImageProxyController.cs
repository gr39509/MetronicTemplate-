using Microsoft.AspNetCore.Mvc;
using NsawaWeb.Services.Services;

namespace NsawaWeb.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ImageProxyController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApiClient _client;
    private readonly AuthService _authService;

    public ImageProxyController(IConfiguration configuration, ApiClient client, AuthService authService)
    {
        _configuration = configuration;
        _client = client;
        _authService = authService;
    }
    
    [HttpGet("GetImage/{imagePath}")]
    public async Task<IActionResult> GetImage(string imagePath)
    {
        var x = _authService.GetTokenAsync();
        var url = $"{_configuration.GetSection("Api:Baseurl").Value}/api/files/{imagePath}";
        // var baseu = _httpClient.BaseAddress;
        
        var response = await _client._httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return NotFound();

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
        var imageBytes = await response.Content.ReadAsByteArrayAsync();

        return File(imageBytes, contentType);
    }
}