using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Commands.LeaveGroup;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class LeaveGroupCommandHandlerTests
{
    [Fact]
    public async Task Handle_SoleAdminWithOtherMembers_ThrowsConflictException()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = memberId, Role = GroupRole.Member });

        var handler = new LeaveGroupCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(new LeaveGroupCommand(groupId), CancellationToken.None));
        Assert.Equal(2, members.Members.Count); // nothing removed
    }

    [Fact]
    public async Task Handle_AdminWithAnotherAdminPresent_Succeeds()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var otherAdminId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = otherAdminId, Role = GroupRole.Admin });

        var handler = new LeaveGroupCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await handler.Handle(new LeaveGroupCommand(groupId), CancellationToken.None);

        Assert.Single(members.Members);
    }

    [Fact]
    public async Task Handle_LastRemainingMember_Succeeds()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var handler = new LeaveGroupCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await handler.Handle(new LeaveGroupCommand(groupId), CancellationToken.None);

        Assert.Empty(members.Members);
    }

    [Fact]
    public async Task Handle_NonMember_ThrowsNotFoundException()
    {
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        var handler = new LeaveGroupCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(Guid.CreateVersion7()));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new LeaveGroupCommand(Guid.CreateVersion7()), CancellationToken.None));
    }
}
