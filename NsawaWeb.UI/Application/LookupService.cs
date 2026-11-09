using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

/// <summary>
/// Reference lists (event types, payment types, networks, roles).
/// They rarely change, so they are cached for the life of the circuit.
/// </summary>
public sealed class LookupService(ApiClient api, ApiRunner runner)
{
    private IReadOnlyList<EventTypeViewModel>? _eventTypes;
    private IReadOnlyList<PaymentTypeViewModel>? _paymentTypes;
    private IReadOnlyList<NetworkType>? _networks;
    private IReadOnlyList<RoleViewModel>? _roles;

    public async Task<Result<IReadOnlyList<EventTypeViewModel>>> EventTypesAsync()
    {
        if (_eventTypes is not null) return Result<IReadOnlyList<EventTypeViewModel>>.Ok(_eventTypes);
        var r = await runner.RunAsync<ICollection<EventTypeViewModel>>(async () => await api.AllAsync(), "load event types");
        if (r.Failed) return Result<IReadOnlyList<EventTypeViewModel>>.Fail(r.Message!);
        _eventTypes = (r.Value ?? []).Where(t => t.Active).OrderBy(t => t.Name).ToList();
        return Result<IReadOnlyList<EventTypeViewModel>>.Ok(_eventTypes);
    }

    public async Task<Result<IReadOnlyList<PaymentTypeViewModel>>> PaymentTypesAsync()
    {
        if (_paymentTypes is not null) return Result<IReadOnlyList<PaymentTypeViewModel>>.Ok(_paymentTypes);
        var r = await runner.RunAsync<ICollection<PaymentTypeViewModel>>(async () => await api.All3Async(), "load payment types");
        if (r.Failed) return Result<IReadOnlyList<PaymentTypeViewModel>>.Fail(r.Message!);
        _paymentTypes = (r.Value ?? []).OrderBy(t => t.Id).ToList();
        return Result<IReadOnlyList<PaymentTypeViewModel>>.Ok(_paymentTypes);
    }

    public async Task<Result<IReadOnlyList<NetworkType>>> NetworksAsync()
    {
        if (_networks is not null) return Result<IReadOnlyList<NetworkType>>.Ok(_networks);
        var r = await runner.RunAsync<ICollection<NetworkType>>(async () => await api.All2Async(), "load mobile networks");
        if (r.Failed) return Result<IReadOnlyList<NetworkType>>.Fail(r.Message!);
        _networks = (r.Value ?? []).ToList();
        return Result<IReadOnlyList<NetworkType>>.Ok(_networks);
    }

    public async Task<Result<IReadOnlyList<RoleViewModel>>> RolesAsync()
    {
        if (_roles is not null) return Result<IReadOnlyList<RoleViewModel>>.Ok(_roles);
        var r = await runner.RunAsync<ICollection<RoleViewModel>>(async () => await api.All4Async(), "load roles");
        if (r.Failed) return Result<IReadOnlyList<RoleViewModel>>.Fail(r.Message!);
        _roles = (r.Value ?? []).OrderBy(x => x.Name).ToList();
        return Result<IReadOnlyList<RoleViewModel>>.Ok(_roles);
    }
}
