using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

/// <summary>Values collected by the event form, shared by create and edit.</summary>
public sealed class EventInput
{
    public string Title { get; set; } = string.Empty;
    public int EventTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Rsvp { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);
    public bool NotifyOrganizer { get; set; } = true;
    public bool NotifyAffiliateOrganizers { get; set; }

    // Not collected by the UI yet; kept so edits round-trip what the API stored.
    public double Latitude { get; set; } = EventsService.DefaultCoordinate;
    public double Longitude { get; set; } = EventsService.DefaultCoordinate;
    public DateTime? ExpiryDate { get; set; }

    public static EventInput From(EventViewModel e) => new()
    {
        Title = e.Title ?? string.Empty,
        EventTypeId = e.EventTypeId,
        Description = e.Description ?? string.Empty,
        Location = e.Location ?? string.Empty,
        Rsvp = e.Rsvp,
        StartDate = e.StartDate.Date,
        EndDate = e.EndDate.Date,
        NotifyOrganizer = e.NotifyOrganizer,
        NotifyAffiliateOrganizers = e.NotifyAffiliateOrganizers,
        Latitude = e.Latitude,
        Longitude = e.Longitude,
        ExpiryDate = e.ExpiryDate?.DateTime
    };
}

/// <summary>An image picked in the browser, already read into memory.</summary>
public sealed record BannerUpload(byte[] Content, string FileName, string ContentType);

/// <summary>What the signed-in user may do on one event.</summary>
public sealed record EventAccess(bool IsOwner, IReadOnlyCollection<string> Roles)
{
    public const string CashierRole = "Cashier";

    public bool IsAffiliate => Roles.Count > 0;
    public bool CanManage => IsOwner;
    public bool CanReceiveDonations => IsOwner || Roles.Contains(CashierRole, StringComparer.OrdinalIgnoreCase);
    public bool CanViewDonations => IsOwner || IsAffiliate;
}

public sealed class EventsService(ApiClient api, ApiRunner runner)
{
    /// <summary>Placeholder the API has always received because the form has no location picker.</summary>
    public const double DefaultCoordinate = 10;

    public async Task<Result<IReadOnlyList<EventViewModel>>> GetMyEventsAsync()
    {
        var result = await runner.RunAsync<ICollection<EventViewModel>>(async () => await api.MyEventsAsync(), "load your events");
        return Sorted(result);
    }

    public async Task<Result<IReadOnlyList<EventViewModel>>> GetAffiliateEventsAsync()
    {
        var result = await runner.RunAsync<ICollection<EventViewModel>>(async () => await api.MyAffiliateEventsAsync(), "load your affiliate events");
        return Sorted(result);
    }

    /// <summary>Works out whether the user owns the event or helps run it, and in which roles.</summary>
    public async Task<Result<EventAccess>> GetAccessAsync(Guid eventId)
    {
        var mine = await GetMyEventsAsync();
        if (mine.Succeeded && mine.Value!.Any(e => e.Id == eventId))
        {
            return Result<EventAccess>.Ok(new EventAccess(true, []));
        }

        var affiliated = await GetAffiliateEventsAsync();
        if (affiliated.Failed && mine.Failed)
        {
            return Result<EventAccess>.Fail(mine.Message!);
        }

        var match = affiliated.Value?.FirstOrDefault(e => e.Id == eventId);
        return Result<EventAccess>.Ok(new EventAccess(false, match?.AffiliationRoles?.ToList() ?? []));
    }

    public Task<Result<EventViewModel>> GetAsync(Guid id) =>
        runner.RunAsync<EventViewModel>(async () => await api.EventAsync(id), "load this event",
            status => status == 404 ? "This event doesn't exist or has been removed." : null);

    public Task<Result<DonationSummaryViewModel>> GetSummaryAsync(Guid id) =>
        runner.RunAsync<DonationSummaryViewModel>(async () => await api.DonationSummaryAsync(id), "load the donation summary");

    public Task<Result<EventViewModel>> CreateAsync(EventInput input, BannerUpload banner) =>
        runner.RunAsync<EventViewModel>(async () => await api.CreateEventAsync(
            title: input.Title.Trim(),
            description: input.Description.Trim(),
            eventTypeId: input.EventTypeId,
            location: input.Location.Trim(),
            longitude: input.Longitude,
            latitude: input.Latitude,
            banner: ToFile(banner),
            startDate: Utc(input.StartDate),
            endDate: Utc(input.EndDate),
            rSVP: input.Rsvp?.Trim() ?? string.Empty,
            notifyOrganizer: input.NotifyOrganizer,
            notifyAffiliateOrganizers: input.NotifyAffiliateOrganizers,
            expiryDate: Utc(input.ExpiryDate ?? input.EndDate.AddYears(1))), "create the event");

    public Task<Result<EventViewModel>> UpdateAsync(Guid id, EventInput input, BannerUpload? newBanner) =>
        runner.RunAsync<EventViewModel>(async () => await api.UpdateEventAsync(
            eventId: id,
            title: input.Title.Trim(),
            description: input.Description.Trim(),
            eventTypeId: input.EventTypeId,
            location: input.Location.Trim(),
            longitude: input.Longitude,
            latitude: input.Latitude,
            // The API requires a banner part; an empty file means "keep the current banner".
            banner: newBanner is null
                ? new FileParameter(new MemoryStream(), "no-change.bin", "application/octet-stream")
                : ToFile(newBanner),
            startDate: Utc(input.StartDate),
            endDate: Utc(input.EndDate),
            rSVP: input.Rsvp?.Trim() ?? string.Empty,
            notifyOrganizer: input.NotifyOrganizer,
            notifyAffiliateOrganizers: input.NotifyAffiliateOrganizers,
            expiryDate: Utc(input.ExpiryDate ?? input.EndDate.AddYears(1))), "save the event");

    public Task<Result> DeactivateAsync(Guid id) =>
        runner.RunAsync(async () => await api.DeactivateAsync(new DeactivateEventDto { EventId = id }), "close the event");

    private static FileParameter ToFile(BannerUpload banner) =>
        new(new MemoryStream(banner.Content), banner.FileName, banner.ContentType);

    // Ghana is UTC+0 all year, so calendar dates are sent as UTC midnight.
    private static DateTimeOffset Utc(DateTime date) =>
        new(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), TimeSpan.Zero);

    private static Result<IReadOnlyList<EventViewModel>> Sorted(Result<ICollection<EventViewModel>> result) =>
        result.Succeeded
            ? Result<IReadOnlyList<EventViewModel>>.Ok((result.Value ?? []).OrderByDescending(e => e.CreatedAt).ToList())
            : Result<IReadOnlyList<EventViewModel>>.Fail(result.Message!);
}
