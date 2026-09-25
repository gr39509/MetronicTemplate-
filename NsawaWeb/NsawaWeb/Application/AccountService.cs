using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

public sealed class AccountService(ApiClient api, ApiRunner runner, AuthService auth)
{
    public async Task<Result> SignInAsync(string phoneNumber, string password)
    {
        var result = await runner.RunAsync<LoginResponseModel>(
            async () => await api.LoginAsync(new LoginDto { PhoneNumber = phoneNumber, Password = password }),
            "sign you in",
            status => status is 400 or 401 ? "That phone number and password don't match an account." : null);

        if (result.Failed)
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(result.Value?.Token))
        {
            return Result.Fail("That phone number and password don't match an account.");
        }

        await auth.LoginAsync(result.Value.Token, phoneNumber);
        return Result.Ok();
    }

    public Task SignOutAsync() => auth.LogoutAsync();

    public Task<Result> RegisterAsync(SignupDto dto) =>
        runner.RunAsync(async () => await api.SignupAsync(dto), "create your account",
            status => status == 409 ? "An account with this phone number already exists. Sign in instead." : null);

    public Task<Result> SetNewPasswordAsync(Guid linkId, string password, string confirmPassword) =>
        runner.RunAsync(async () => await api.SetNewPasswordAsync(new SetNewPasswordDto
        {
            Id = linkId,
            Password = password,
            ConfirmPassword = confirmPassword
        }), "set your password");

    public Task<Result> ChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword) =>
        runner.RunAsync(async () => await api.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = currentPassword,
            Password = newPassword,
            ConfirmPassword = confirmPassword
        }), "change your password",
            status => status == 400 ? "Your current password is incorrect, or the new password isn't allowed." : null);
}
