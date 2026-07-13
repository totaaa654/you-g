using YouG.Application.Features.Groups.Commands.CreateGroup;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Groups;

public class CreateGroupCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesGroup_WithCallerAsSoleAdmin()
    {
        var groups = new FakeGroupRepository();
        var members = new FakeGroupMemberRepository(groups);
        var callerId = Guid.CreateVersion7();
        var handler = new CreateGroupCommandHandler(
            groups, members, new FakeUnitOfWork(), new FakeCurrentUserService(callerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.Handle(new CreateGroupCommand("Friday Crew", "Weekly hangout"), CancellationToken.None);

        Assert.Equal("Friday Crew", result.Name);
        Assert.Equal(1, result.MemberCount);
        Assert.Single(groups.Groups);

        var membership = Assert.Single(members.Members);
        Assert.Equal(callerId, membership.UserId);
        Assert.Equal(GroupRole.Admin, membership.Role);
    }
}
