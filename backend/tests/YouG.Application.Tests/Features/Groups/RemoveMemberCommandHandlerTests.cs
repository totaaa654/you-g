using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Commands.RemoveMember;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class RemoveMemberCommandHandlerTests
{
    [Fact]
    public async Task Handle_CallerNotAdmin_ThrowsForbiddenException()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var targetId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = callerId, Role = GroupRole.Member });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = targetId, Role = GroupRole.Member });

        var handler = new RemoveMemberCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(callerId));

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(new RemoveMemberCommand(groupId, targetId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AdminRemovingSelf_ThrowsConflictException()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var handler = new RemoveMemberCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RemoveMemberCommand(groupId, adminId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AdminRemovingAnotherMember_Succeeds()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var targetId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = targetId, Role = GroupRole.Member });

        var handler = new RemoveMemberCommandHandler(members, new FakeUnitOfWork(), new FakeCurrentUserService(adminId));

        await handler.Handle(new RemoveMemberCommand(groupId, targetId), CancellationToken.None);

        Assert.Single(members.Members);
    }
}
