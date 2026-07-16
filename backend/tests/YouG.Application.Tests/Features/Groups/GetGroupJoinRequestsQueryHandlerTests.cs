using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Queries.GetGroupJoinRequests;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class GetGroupJoinRequestsQueryHandlerTests
{
    [Fact]
    public async Task Handle_NonAdminCaller_ThrowsForbiddenException()
    {
        var groupId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = memberId, Role = GroupRole.Member });

        var handler = new GetGroupJoinRequestsQueryHandler(
            members, new FakeGroupJoinRequestRepository(), new FakeUserRepository(), new FakeCurrentUserService(memberId));

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(new GetGroupJoinRequestsQuery(groupId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AdminCaller_ReturnsOnlyPendingRequests()
    {
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var requesterId = Guid.CreateVersion7();
        var declinedRequesterId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin });

        var users = new FakeUserRepository();
        var requester = new User { Email = "a@b.com", Username = "requester", FriendCode = "YG-000001", DisplayName = "Requester", TimeZoneId = "UTC" };
        users.Users.Add(requester);
        users.Users.Add(new User { Email = "c@d.com", Username = "declined", FriendCode = "YG-000002", DisplayName = "Declined", TimeZoneId = "UTC", Id = declinedRequesterId });

        var joinRequests = new FakeGroupJoinRequestRepository();
        joinRequests.Requests.Add(new GroupJoinRequest
        {
            GroupId = groupId, UserId = requester.Id, Status = GroupJoinRequestStatus.Pending, CreatedAt = DateTimeOffset.UtcNow
        });
        joinRequests.Requests.Add(new GroupJoinRequest
        {
            GroupId = groupId, UserId = declinedRequesterId, Status = GroupJoinRequestStatus.Declined, CreatedAt = DateTimeOffset.UtcNow
        });

        var handler = new GetGroupJoinRequestsQueryHandler(members, joinRequests, users, new FakeCurrentUserService(adminId));

        var result = await handler.Handle(new GetGroupJoinRequestsQuery(groupId), CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal(requester.Id, dto.Profile.Id);
    }
}
