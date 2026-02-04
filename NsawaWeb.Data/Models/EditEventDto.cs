using NsawaWeb.Services.Services;

namespace NsawaWeb.Data.Models;

public class EditEventDto : CreateEventDto
{
    public Guid Id { get; set; }
    public string BannerUrl { get; set; } = string.Empty;
    public FileParameter? Banner { get; set; }
}