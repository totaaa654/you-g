using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Queries.GetGroupById;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class GetGroupByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_NonMember_ThrowsNotFoundException()
    {
        var now = DateTimeOffset.UtcNow;
        var groups = new FakeGroupRepository();
        var group = new Group { Name = "Friday Crew", CreatedByUserId = Guid.CreateVersion7(), CreatedAt = now, UpdatedAt = now };
        groups.Groups.Add(group);
        var members = new FakeGroupMemberRepository(groups);

        var handler = new GetGroupByIdQueryHandler(groups, members, new FakeCurrentUserService(Guid.CreateVersion7()));

        // 404, not 403 — a non-member shouldn't be able to distinguish "doesn't exist" from
        // "exists but I can't see it" (docs/04-API-DESIGN.md Section 1.7).
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Member_ReturnsGroup()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var groups = new FakeGroupRepository();
        var group = new Group { Name = "Friday Crew", CreatedByUserId = callerId, CreatedAt = now, UpdatedAt = now };
        groups.Groups.Add(group);
        var members = new FakeGroupMemberRepository(groups);
        members.Members.Add(new GroupMember { GroupId = group.Id, UserId = callerId, Role = GroupRole.Admin, JoinedAt = now });

        var handler = new GetGroupByIdQueryHandler(groups, members, new FakeCurrentUserService(callerId));

        var result = await handler.Handle(new GetGroupByIdQuery(group.Id), CancellationToken.None);

        Assert.Equal(group.Id, result.Id);
        Assert.Equal(1, result.MemberCount);
    }
}
