namespace NsawaWeb.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNsawaApplication(this IServiceCollection services)
    {
        services.AddScoped<ApiRunner>();
        services.AddScoped<AccountService>();
        services.AddScoped<EventsService>();
        services.AddScoped<DonationsService>();
        services.AddScoped<WithdrawalsService>();
        services.AddScoped<GroupsService>();
        services.AddScoped<AffiliatesService>();
        services.AddScoped<LookupService>();
        services.AddScoped<ToastService>();
        services.AddSingleton<QrCodeService>();
        return services;
    }
}
