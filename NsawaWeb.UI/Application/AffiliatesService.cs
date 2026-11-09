using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

public sealed class AffiliateInput
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? Address { get; set; }
    public HashSet<string> RoleIds { get; set; } = [];
    public bool NotifyDonation { get; set; } = true;
    public bool PushNotifyDonation { get; set; } = true;

    public static AffiliateInput From(EventAffiliateViewModel a) => new()
    {
        FullName = a.FullName ?? string.Empty,
        PhoneNumber = a.PhoneNumber ?? string.Empty,
        OrganizationName = a.OrganizationName,
        Address = a.Address,
        RoleIds = [.. a.RoleIds ?? []],
        NotifyDonation = a.NotifyDonation,
        PushNotifyDonation = a.PushNotifyDonation
    };
}

public sealed class AffiliatesService(ApiClient api, ApiRunner runner)
{
    private const string DuplicatePhone = "Someone with this phone number is already an affiliate of this event.";

    public async Task<Result<IReadOnlyList<EventAffiliateViewModel>>> ListAsync(Guid eventId)
    {
        var result = await runner.RunAsync<ICollection<EventAffiliateViewModel>>(async () => await api.EventAffiliateAsync(eventId), "load affiliates");
        return result.Succeeded
            ? Result<IReadOnlyList<EventAffiliateViewModel>>.Ok((result.Value ?? []).OrderByDescending(a => a.CreatedAt).ToList())
            : Result<IReadOnlyList<EventAffiliateViewModel>>.Fail(result.Message!);
    }

    public Task<Result> AddAsync(Guid eventId, AffiliateInput input) =>
        runner.RunAsync(async () => await api.CreateAsync(new CreateEventAffiliateDto
        {
            EventId = eventId,
            FullName = input.FullName.Trim(),
            PhoneNumber = input.PhoneNumber.Trim(),
            OrganizationName = input.OrganizationName?.Trim() ?? string.Empty,
            Address = input.Address?.Trim() ?? string.Empty,
            RoleIds = [.. input.RoleIds],
            NotifyDonation = input.NotifyDonation,
            PushNotifyDonation = input.PushNotifyDonation
        }), "add the affiliate", status => status == 409 ? DuplicatePhone : null);

    public Task<Result> UpdateAsync(Guid eventId, Guid affiliateId, AffiliateInput input) =>
        runner.RunAsync(async () => await api.UpdatePUTAsync(new UpdateEventAffiliateDto
        {
            EventAffiliateId = affiliateId,
            EventId = eventId,
            FullName = input.FullName.Trim(),
            PhoneNumber = input.PhoneNumber.Trim(),
            OrganizationName = input.OrganizationName?.Trim() ?? string.Empty,
            Address = input.Address?.Trim() ?? string.Empty,
            RoleIds = [.. input.RoleIds],
            NotifyDonation = input.NotifyDonation,
            PushNotifyDonation = input.PushNotifyDonation
        }), "save the affiliate", status => status == 409 ? DuplicatePhone : null);

    public Task<Result> RemoveAsync(Guid affiliateId) =>
        runner.RunAsync(async () => await api.DeleteDELETEAsync(new DeleteEventAffiliateDto { EventAffiliateId = affiliateId }), "remove the affiliate");
}
