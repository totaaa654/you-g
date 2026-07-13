using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Groups.Commands.JoinGroupViaInvite;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class JoinGroupViaInviteCommandHandlerTests
{
    private static (JoinGroupViaInviteCommandHandler Handler, FakeGroupMemberRepository Members, Group Group) CreateHandler(
        DateTimeOffset now, Guid callerId, string code, DateTimeOffset expiresAt)
    {
        var groups = new FakeGroupRepository();
        var group = new Group { Name = "Friday Crew", CreatedByUserId = Guid.CreateVersion7(), CreatedAt = now, UpdatedAt = now };
        groups.Groups.Add(group);

        var members = new FakeGroupMemberRepository(groups);
        var links = new FakeGroupInviteLinkRepository();
        links.Links.Add(new GroupInviteLink
        {
            GroupId = group.Id,
            Code = code,
            CreatedByUserId = Guid.CreateVersion7(),
            ExpiresAt = expiresAt,
            CreatedAt = now
        });

        var handler = new JoinGroupViaInviteCommandHandler(
            groups, members, links, new FakeUnitOfWork(), new FakeCurrentUserService(callerId), new FakeDateTimeProvider(now));

        return (handler, members, group);
    }

    [Fact]
    public async Task Handle_ValidCode_AddsMembership()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, members, group) = CreateHandler(now, callerId, "ABC12345", now.AddDays(1));

        var result = await handler.Handle(new JoinGroupViaInviteCommand("ABC12345"), CancellationToken.None);

        Assert.Equal(group.Id, result.Id);
        var membership = Assert.Single(members.Members);
        Assert.Equal(callerId, membership.UserId);
        Assert.Equal(GroupRole.Member, membership.Role);
    }

    [Fact]
    public async Task Handle_ExpiredCode_ThrowsNotFoundException()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, _, _) = CreateHandler(now, Guid.CreateVersion7(), "EXPIRED1", now.AddDays(-1));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new JoinGroupViaInviteCommand("EXPIRED1"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownCode_ThrowsNotFoundException()
    {
        var now = DateTimeOffset.UtcNow;
        var (handler, _, _) = CreateHandler(now, Guid.CreateVersion7(), "REALCODE", now.AddDays(1));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new JoinGroupViaInviteCommand("WRONGCODE"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyMember_IsIdempotent()
    {
        var now = DateTimeOffset.UtcNow;
        var callerId = Guid.CreateVersion7();
        var (handler, members, group) = CreateHandler(now, callerId, "REUSED12", now.AddDays(1));
        members.Members.Add(new GroupMember { GroupId = group.Id, UserId = callerId, Role = GroupRole.Member, JoinedAt = now });

        await handler.Handle(new JoinGroupViaInviteCommand("REUSED12"), CancellationToken.None);

        Assert.Single(members.Members); // still just one membership, not duplicated
    }
}
