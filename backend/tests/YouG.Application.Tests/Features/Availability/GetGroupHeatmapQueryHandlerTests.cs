using YouG.Application.Features.Availability.Queries.GetGroupHeatmap;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Availability;

public class GetGroupHeatmapQueryHandlerTests
{
    [Fact]
    public async Task Handle_CountsOnlyAvailableStatus_NotMaybe()
    {
        var groupId = Guid.CreateVersion7();
        var userA = Guid.CreateVersion7();
        var userB = Guid.CreateVersion7();
        var date = new DateOnly(2026, 7, 13);

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = userA, Role = GroupRole.Admin });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = userB, Role = GroupRole.Member });

        var instances = new FakeAvailabilityInstanceRepository();
        instances.Instances.Add(new AvailabilityInstance { UserId = userA, Date = date, Daypart = Daypart.Evening, Status = AvailabilityStatus.Available });
        instances.Instances.Add(new AvailabilityInstance { UserId = userB, Date = date, Daypart = Daypart.Evening, Status = AvailabilityStatus.Maybe });

        var handler = new GetGroupHeatmapQueryHandler(members, instances, new FakeCurrentUserService(userA));

        var result = await handler.Handle(new GetGroupHeatmapQuery(groupId, date, date), CancellationToken.None);

        var cell = Assert.Single(result.Cells);
        Assert.Equal(1, cell.AvailableCount); // only userA (Available) counted, not userB (Maybe)
        Assert.Equal(2, result.TotalMembers);
    }
}
