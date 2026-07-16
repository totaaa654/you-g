using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Commands.JoinGroupViaInvite;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class JoinGroupViaInviteCommandHandlerTests
{
    private static (
        JoinGroupViaInviteCommandHandler Handler,
        FakeGroupMemberRepository Members,
        FakeGroupJoinRequestRepository JoinRequests,
        Group Group) CreateHandler(
        DateTimeOffset now, Guid callerId, string code, DateTimeOffset expiresAt, bool creatorIsAdmin = true)
    {
        var groups = new FakeGroupRepository();
        var group = new Group { Name = "Friday Crew", CreatedByUserId = Guid.CreateVersion7(), CreatedAt = now, UpdatedAt = now };
        groups.Groups.Add(group);

        var members = new FakeGroupMemberRepository(groups);
        var creatorId = Guid.CreateVersion7();
        if (creatorIsAdmin)
        {
            members.Members.Add(new GroupMember { GroupId = group.Id, UserId = creatorId, Role = GroupRole.Admin, JoinedAt = now });
        }

        var links = new FakeGroupInviteLinkRepository();
        links.Links.Add(new GroupInviteLink
        {
            GroupId = group.Id,
            Code = code,
            CreatedByUserId = creatorId,
            ExpiresAt = expiresAt,
            CreatedAt = now
        });

        var joinRequests = new FakeGroupJoinRequestRepository();

        var handler = new JoinGroupViaInviteCommandHandler(
            groups, members, links, joinRequests, new FakeUnitOfWork(), new FakeCurrentUserService(callerId), new FakeDateTimeProvider(now));

        return (handler, members, joinRequests, group);
    }

    [Fact]
    public async Task Handle_AdminCreatedLink_JoinsInstantly()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, members, joinRequests, group) = CreateHandler(now, callerId, "ABC12345", now.AddDays(1));

        var result = await handler.Handle(new JoinGroupViaInviteCommand("ABC12345"), CancellationToken.None);

        Assert.True(result.Joined);
        Assert.Equal(group.Id, result.Group!.Id);
        var membership = Assert.Single(members.Members, m => m.UserId == callerId);
        Assert.Equal(GroupRole.Member, membership.Role);
        Assert.Empty(joinRequests.Requests);
    }

    [Fact]
    public async Task Handle_ExpiredCode_ThrowsNotFoundException()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, _, _, _) = CreateHandler(now, Guid.CreateVersion7(), "EXPIRED1", now.AddDays(-1));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new JoinGroupViaInviteCommand("EXPIRED1"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownCode_ThrowsNotFoundException()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, _, _, _) = CreateHandler(now, Guid.CreateVersion7(), "REALCODE", now.AddDays(1));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new JoinGroupViaInviteCommand("WRONGCODE"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyMember_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, members, _, group) = CreateHandler(now, callerId, "REUSED12", now.AddDays(1));
        members.Members.Add(new GroupMember { GroupId = group.Id, UserId = callerId, Role = GroupRole.Member, JoinedAt = now });

        var result = await handler.Handle(new JoinGroupViaInviteCommand("REUSED12"), CancellationToken.None);

        Assert.True(result.Joined);
        Assert.Single(members.Members, m => m.UserId == callerId); // still just one membership, not duplicated
    }

    [Fact]
    public async Task Handle_MemberCreatedLink_CreatesPendingJoinRequestInsteadOfMembership()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, members, joinRequests, group) =
            CreateHandler(now, callerId, "MEMBER01", now.AddDays(1), creatorIsAdmin: false);

        var result = await handler.Handle(new JoinGroupViaInviteCommand("MEMBER01"), CancellationToken.None);

        Assert.False(result.Joined);
        Assert.Null(result.Group);
        Assert.DoesNotContain(members.Members, m => m.UserId == callerId);
        var request = Assert.Single(joinRequests.Requests);
        Assert.Equal(group.Id, request.GroupId);
        Assert.Equal(callerId, request.UserId);
        Assert.Equal(GroupJoinRequestStatus.Pending, request.Status);
    }

    [Fact]
    public async Task Handle_MemberCreatedLink_ReusedWhilePending_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, _, joinRequests, group) = CreateHandler(now, callerId, "MEMBER02", now.AddDays(1), creatorIsAdmin: false);
        joinRequests.Requests.Add(new GroupJoinRequest
        {
            GroupId = group.Id, UserId = callerId, Status = GroupJoinRequestStatus.Pending, CreatedAt = now
        });

        await handler.Handle(new JoinGroupViaInviteCommand("MEMBER02"), CancellationToken.None);

        Assert.Single(joinRequests.Requests);
    }

    [Fact]
    public async Task Handle_MemberCreatedLink_ReusedAfterDecline_ReopensRequest()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, _, joinRequests, group) = CreateHandler(now, callerId, "MEMBER03", now.AddDays(1), creatorIsAdmin: false);
        joinRequests.Requests.Add(new GroupJoinRequest
        {
            GroupId = group.Id,
            UserId = callerId,
            Status = GroupJoinRequestStatus.Declined,
            CreatedAt = now.AddDays(-1),
            RespondedAt = now.AddHours(-1)
        });

        await handler.Handle(new JoinGroupViaInviteCommand("MEMBER03"), CancellationToken.None);

        var request = Assert.Single(joinRequests.Requests);
        Assert.Equal(GroupJoinRequestStatus.Pending, request.Status);
        Assert.Null(request.RespondedAt);
    }
}
