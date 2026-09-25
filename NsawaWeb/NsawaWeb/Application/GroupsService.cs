using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

public sealed class GroupsService(ApiClient api, ApiRunner runner)
{
    public async Task<Result<IReadOnlyList<EventGroupViewModel>>> ListAsync(Guid eventId)
    {
        var result = await runner.RunAsync<ICollection<EventGroupViewModel>>(async () => await api.PerEventAsync(eventId), "load groups");
        return result.Succeeded
            ? Result<IReadOnlyList<EventGroupViewModel>>.Ok((result.Value ?? []).OrderBy(g => g.GroupName).ToList())
            : Result<IReadOnlyList<EventGroupViewModel>>.Fail(result.Message!);
    }

    public Task<Result<EventGroupViewModel>> CreateAsync(Guid eventId, string name) =>
        runner.RunAsync<EventGroupViewModel>(async () => await api.Create2Async(new CreateEventGroupDto { EventId = eventId, GroupName = name.Trim() }),
            "add the group", status => status == 409 ? "A group with this name already exists." : null);

    public Task<Result<EventGroupViewModel>> RenameAsync(Guid eventId, Guid groupId, string name) =>
        runner.RunAsync<EventGroupViewModel>(async () => await api.UpdatePOSTAsync(new UpdateEventGroupDto { EventId = eventId, GroupId = groupId, GroupName = name.Trim() }),
            "rename the group", status => status == 409 ? "A group with this name already exists." : null);

    public Task<Result> DeleteAsync(Guid groupId) =>
        runner.RunAsync(async () => await api.DeletePOSTAsync(new DeleteEventGroupDto { GroupId = groupId }), "delete the group");
}
