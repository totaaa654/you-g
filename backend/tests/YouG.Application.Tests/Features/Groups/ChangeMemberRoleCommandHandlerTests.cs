using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Commands.ChangeMemberRole;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class ChangeMemberRoleCommandHandlerTests
{
    [Fact]
    public async Task Handle_DemotingSoleAdmin_ThrowsConflictException()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var handler = new ChangeMemberRoleCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new ChangeMemberRoleCommand(groupId, adminId, GroupRole.Member), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PromotingMemberToAdmin_Succeeds()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });
        var target = new GroupMember { GroupId = groupId, UserId = memberId, Role = GroupRole.Member };
        members.Members.Add(target);

        var handler = new ChangeMemberRoleCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await handler.Handle(new ChangeMemberRoleCommand(groupId, memberId, GroupRole.Admin), CancellationToken.None);

        Assert.Equal(GroupRole.Admin, target.Role);
    }
}
