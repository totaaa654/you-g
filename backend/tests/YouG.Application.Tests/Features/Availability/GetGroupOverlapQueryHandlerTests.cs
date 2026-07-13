using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Availability.Queries.GetGroupOverlap;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Availability;

public class GetGroupOverlapQueryHandlerTests
{
    private static DateOnly Date(int daysFromMonday2026_07_13) => new DateOnly(2026, 7, 13).AddDays(daysFromMonday2026_07_13);

    [Fact]
    public async Task Handle_RanksWindowsByAvailableCountDescending()
    {
        var groupId = Guid.CreateVersion7();
        var userA = Guid.CreateVersion7();
        var userB = Guid.CreateVersion7();
        var userC = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = userA, Role = GroupRole.Admin });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = userB, Role = GroupRole.Member });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = userC, Role = GroupRole.Member });

        var instances = new FakeAvailabilityInstanceRepository();
        // Monday evening: only userA free.
        instances.Instances.Add(new AvailabilityInstance { UserId = userA, Date = Date(0), Daypart = Daypart.Evening, Status = AvailabilityStatus.Available });
        // Tuesday evening: userA and userB free.
        instances.Instances.Add(new AvailabilityInstance { UserId = userA, Date = Date(1), Daypart = Daypart.Evening, Status = AvailabilityStatus.Available });
        instances.Instances.Add(new AvailabilityInstance { UserId = userB, Date = Date(1), Daypart = Daypart.Evening, Status = AvailabilityStatus.Available });

        var handler = new GetGroupOverlapQueryHandler(members, instances, new FakeCurrentUserService(userA));

        var result = await handler.Handle(
            new GetGroupOverlapQuery(groupId, Date(0), Date(1), WeekendOnly: false, PreferredDayparts: null),
            CancellationToken.None);

        Assert.Equal(2, result.Windows.Count);
        Assert.Equal(2, result.Windows[0].AvailableCount); // Tuesday ranked first
        Assert.Equal(Date(1), result.Windows[0].Date);
        Assert.Equal(1, result.Windows[1].AvailableCount); // Monday ranked second
        Assert.Equal(3, result.Windows[0].TotalMembers);
    }

    [Fact]
    public async Task Handle_WeekendOnlyFilter_ExcludesWeekdays()
    {
        var groupId = Guid.CreateVersion7();
        var userA = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = userA, Role = GroupRole.Admin });

        var instances = new FakeAvailabilityInstanceRepository();
        instances.Instances.Add(new AvailabilityInstance { UserId = userA, Date = Date(0), Daypart = Daypart.Evening, Status = AvailabilityStatus.Available }); // Monday
        instances.Instances.Add(new AvailabilityInstance { UserId = userA, Date = Date(5), Daypart = Daypart.Evening, Status = AvailabilityStatus.Available }); // Saturday

        var handler = new GetGroupOverlapQueryHandler(members, instances, new FakeCurrentUserService(userA));

        var result = await handler.Handle(
            new GetGroupOverlapQuery(groupId, Date(0), Date(6), WeekendOnly: true, PreferredDayparts: null),
            CancellationToken.None);

        var window = Assert.Single(result.Windows);
        Assert.Equal(Date(5), window.Date);
    }

    [Fact]
    public async Task Handle_NonMember_ThrowsNotFoundException()
    {
        var groupId = Guid.CreateVersion7();
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        var instances = new FakeAvailabilityInstanceRepository();

        var handler = new GetGroupOverlapQueryHandler(members, instances, new FakeCurrentUserService(Guid.CreateVersion7()));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new GetGroupOverlapQuery(groupId, Date(0), Date(1), false, null), CancellationToken.None));
    }
}
