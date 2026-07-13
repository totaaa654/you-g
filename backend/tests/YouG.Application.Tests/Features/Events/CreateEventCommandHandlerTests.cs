using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Events.Commands.CreateEvent;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Events;

public class CreateEventCommandHandlerTests
{
    private static (CreateEventCommandHandler Handler, FakeEventRepository Events) CreateHandler(Guid groupId, Guid callerId)
    {
        var events = new FakeEventRepository();
        var timeOptions = new FakeEventTimeOptionRepository();
        var locationOptions = new FakeEventLocationOptionRepository();
        var groupMembers = new FakeGroupMemberRepository(new FakeGroupRepository());
        groupMembers.Members.Add(new GroupMember { GroupId = groupId, UserId = callerId, Role = GroupRole.Member });

        var handler = new CreateEventCommandHandler(
            events, timeOptions, locationOptions, groupMembers, new FakeUnitOfWork(),
            new FakeCurrentUserService(callerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        return (handler, events);
    }

    [Fact]
    public async Task Handle_WithBothTimeAndLocation_CreatesConfirmedEvent()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var (handler, events) = CreateHandler(groupId, callerId);

        var start = DateTimeOffset.UtcNow.AddDays(3);
        var result = await handler.Handle(
            new CreateEventCommand(groupId, "Board Games", null, null, start, start.AddHours(2), "Maya's Place", null, -33.8, 151.2),
            CancellationToken.None);

        Assert.Equal(EventStatus.Confirmed, result.Status);
        Assert.NotNull(result.ConfirmedTimeOptionId);
        Assert.NotNull(result.ConfirmedLocationOptionId);
        Assert.Single(events.Events);
    }

    [Fact]
    public async Task Handle_WithNeitherTimeNorLocation_CreatesProposedEvent()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var (handler, _) = CreateHandler(groupId, callerId);

        var result = await handler.Handle(
            new CreateEventCommand(groupId, "Board Games", null, null, null, null, null, null, null, null),
            CancellationToken.None);

        Assert.Equal(EventStatus.Proposed, result.Status);
        Assert.Null(result.ConfirmedTimeOptionId);
        Assert.Null(result.ConfirmedLocationOptionId);
    }

    [Fact]
    public async Task Handle_WithOnlyTime_CreatesProposedEventWithCandidateTimeOption()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var (handler, _) = CreateHandler(groupId, callerId);

        var start = DateTimeOffset.UtcNow.AddDays(3);
        var result = await handler.Handle(
            new CreateEventCommand(groupId, "Board Games", null, null, start, start.AddHours(2), null, null, null, null),
            CancellationToken.None);

        // A candidate time option exists, but the event isn't Confirmed until a location
        // is also chosen and /confirm is called.
        Assert.Equal(EventStatus.Proposed, result.Status);
        Assert.Null(result.ConfirmedTimeOptionId);
    }

    [Fact]
    public async Task Handle_NonMember_ThrowsNotFoundException()
    {
        var groupId = Guid.CreateVersion7();
        var events = new FakeEventRepository();
        var timeOptions = new FakeEventTimeOptionRepository();
        var locationOptions = new FakeEventLocationOptionRepository();
        var groupMembers = new FakeGroupMemberRepository(new FakeGroupRepository());

        var handler = new CreateEventCommandHandler(
            events, timeOptions, locationOptions, groupMembers, new FakeUnitOfWork(),
            new FakeCurrentUserService(Guid.CreateVersion7()), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new CreateEventCommand(groupId, "Board Games", null, null, null, null, null, null, null, null),
            CancellationToken.None));
    }
}
