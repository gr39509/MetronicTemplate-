using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

public sealed class DonationsService(ApiClient api, ApiRunner runner)
{
    public async Task<Result<IReadOnlyList<DonationViewModel>>> ListAsync(Guid eventId, DateTime? from, DateTime? to, string? paymentType)
    {
        var result = await runner.RunAsync<ICollection<DonationViewModel>>(async () => await api.DonationsAsync(
            eventId,
            from is null ? null : new DateTimeOffset(from.Value.Date, TimeSpan.Zero),
            to is null ? null : new DateTimeOffset(to.Value.Date.AddDays(1).AddSeconds(-1), TimeSpan.Zero),
            string.IsNullOrWhiteSpace(paymentType) ? null! : paymentType), "load donations");

        return result.Succeeded
            ? Result<IReadOnlyList<DonationViewModel>>.Ok((result.Value ?? []).OrderByDescending(d => d.DateDonated).ToList())
            : Result<IReadOnlyList<DonationViewModel>>.Fail(result.Message!);
    }

    /// <summary>Donation recorded by an organiser or cashier at the event.</summary>
    public Task<Result<DonationResponseModel>> ReceiveAsync(DonationViaPOSDto dto) =>
        runner.RunAsync<DonationResponseModel>(async () => await api.DonateViaPOSAsync(dto), "record the donation");

    // Public donor flow
    public Task<Result> RequestDonorCodeAsync(string phoneNumber) =>
        runner.RunAsync(async () => await api.RequestOTP2Async(new RequestOTPDto { PhoneNumber = phoneNumber }), "send the code");

    public Task<Result<DonorViewModel>> VerifyDonorCodeAsync(string phoneNumber, string code, string? fullName) =>
        runner.RunAsync<DonorViewModel>(async () => await api.VerifyOTP2Async(new VerifyOTPDto
        {
            PhoneNumber = phoneNumber,
            Otp = code,
            FullName = fullName ?? string.Empty
        }), "check the code",
            status => status is 400 or 401 ? "That code isn't right or has expired. Check it or send a new one." : null);

    public Task<Result<DonationResponseModel>> DonateAsync(DonationDto dto) =>
        runner.RunAsync<DonationResponseModel>(async () => await api.DonateAsync(dto), "start your donation");
}
