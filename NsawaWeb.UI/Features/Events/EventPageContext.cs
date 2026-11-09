using NsawaWeb.Application;
using NsawaWeb.Services.Services;

namespace NsawaWeb.Features.Events;

/// <summary>Handed to each event tab: the loaded event, the user's access, and a way to refresh the header.</summary>
public sealed record EventPageContext(EventViewModel Event, EventAccess Access, Func<Task> ReloadAsync);
