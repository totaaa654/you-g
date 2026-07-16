using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Commands.RespondToGroupJoinRequest;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class RespondToGroupJoinRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_Accepted_CreatesMembershipAndUpdatesStatus()
    {
        var now = DateTimeOffset.UtcNow;
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var requesterId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var joinRequests = new FakeGroupJoinRequestRepository();
        var request = new GroupJoinRequest { GroupId = groupId, UserId = requesterId, Status = GroupJoinRequestStatus.Pending, CreatedAt = now };
        joinRequests.Requests.Add(request);

        var handler = new RespondToGroupJoinRequestCommandHandler(
            members, joinRequests, new FakeUnitOfWork(), new FakeCurrentUserService(adminId), new FakeDateTimeProvider(now));

        await handler.Handle(new RespondToGroupJoinRequestCommand(groupId, request.Id, GroupJoinRequestStatus.Accepted), CancellationToken.None);

        Assert.Equal(GroupJoinRequestStatus.Accepted, request.Status);
        Assert.Equal(now, request.RespondedAt);
        var membership = Assert.Single(members.Members, m => m.UserId == requesterId);
        Assert.Equal(GroupRole.Member, membership.Role);
    }

    [Fact]
    public async Task Handle_Declined_DoesNotCreateMembership()
    {
        var now = DateTimeOffset.UtcNow;
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var requesterId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var joinRequests = new FakeGroupJoinRequestRepository();
        var request = new GroupJoinRequest { GroupId = groupId, UserId = requesterId, Status = GroupJoinRequestStatus.Pending, CreatedAt = now };
        joinRequests.Requests.Add(request);

        var handler = new RespondToGroupJoinRequestCommandHandler(
            members, joinRequests, new FakeUnitOfWork(), new FakeCurrentUserService(adminId), new FakeDateTimeProvider(now));

        await handler.Handle(new RespondToGroupJoinRequestCommand(groupId, request.Id, GroupJoinRequestStatus.Declined), CancellationToken.None);

        Assert.Equal(GroupJoinRequestStatus.Declined, request.Status);
        Assert.DoesNotContain(members.Members, m => m.UserId == requesterId);
    }

    [Fact]
    public async Task Handle_NonAdminCaller_ThrowsForbiddenException()
    {
        var groupId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = memberId, Role = GroupRole.Member });

        var joinRequests = new FakeGroupJoinRequestRepository();
        var request = new GroupJoinRequest { GroupId = groupId, UserId = Guid.CreateVersion7(), Status = GroupJoinRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow };
        joinRequests.Requests.Add(request);

        var handler = new RespondToGroupJoinRequestCommandHandler(
            members, joinRequests, new FakeUnitOfWork(), new FakeCurrentUserService(memberId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(new RespondToGroupJoinRequestCommand(groupId, request.Id, GroupJoinRequestStatus.Accepted), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyResponded_ThrowsConflictException()
    {
        var now = DateTimeOffset.UtcNow;
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var joinRequests = new FakeGroupJoinRequestRepository();
        var request = new GroupJoinRequest
        {
            GroupId = groupId, UserId = Guid.CreateVersion7(), Status = GroupJoinRequestStatus.Accepted, CreatedAt = now, RespondedAt = now
        };
        joinRequests.Requests.Add(request);

        var handler = new RespondToGroupJoinRequestCommandHandler(
            members, joinRequests, new FakeUnitOfWork(), new FakeCurrentUserService(adminId), new FakeDateTimeProvider(now));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RespondToGroupJoinRequestCommand(groupId, request.Id, GroupJoinRequestStatus.Declined), CancellationToken.None));
    }
}
