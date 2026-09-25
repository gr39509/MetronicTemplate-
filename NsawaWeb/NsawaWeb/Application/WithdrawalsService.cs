using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

public sealed class WithdrawalsService(ApiClient api, ApiRunner runner)
{
    public Task<Result<MoMoNumberVerificationResponse>> VerifyWalletAsync(string network, string phoneNumber) =>
        runner.RunAsync<MoMoNumberVerificationResponse>(async () => await api.VerifyCustomerAsync(network, phoneNumber), "check this wallet");

    public Task<Result> WithdrawAsync(WithdrawFundsDto dto) =>
        runner.RunAsync(async () => await api.WithdrawFundsAsync(dto), "send the withdrawal");
}
