using System.ComponentModel.DataAnnotations;
using NsawaWeb.Services.Services;

namespace NsawaWeb.Data.Models;

public class CreateEventDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required] public int EventTypeId { get; set; } = 1;
    public string Location { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [Required]
    public FileParameter Banner { get; set; } = null!;
    [Required]
    public DateTime? StartDate { get; set; } = DateTime.Today;
    [Required]
    public DateTime? EndDate { get; set; } =  DateTime.Now;
    [Required]
    public string Rsvp { get; set; } = string.Empty;

    public bool NotifyOrganizer { get; set; } = true;
    public bool NotifyAffiliateOrganizers { get; set; } = true;
}