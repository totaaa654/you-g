using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Events.Commands.ConfirmEvent;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Events;

public class ConfirmEventCommandHandlerTests
{
    private static (Event Event, EventTimeOption TimeOption, EventLocationOption LocationOption, FakeEventRepository Events,
        FakeEventTimeOptionRepository TimeOptions, FakeEventLocationOptionRepository LocationOptions, FakeGroupMemberRepository Members)
        Setup(Guid organizerId)
    {
        var groupId = Guid.CreateVersion7();
        var @event = new Event { GroupId = groupId, CreatedByUserId = organizerId, Title = "Board Games", Status = EventStatus.Proposed };
        var timeOption = new EventTimeOption { EventId = @event.Id, StartUtc = DateTimeOffset.UtcNow, EndUtc = DateTimeOffset.UtcNow.AddHours(2), ProposedByUserId = organizerId };
        var locationOption = new EventLocationOption { EventId = @event.Id, Name = "Cafe", Latitude = 0, Longitude = 0, ProposedByUserId = organizerId };

        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var timeOptions = new FakeEventTimeOptionRepository();
        timeOptions.Options.Add(timeOption);
        var locationOptions = new FakeEventLocationOptionRepository();
        locationOptions.Options.Add(locationOption);
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = organizerId, Role = GroupRole.Member });

        return (@event, timeOption, locationOption, events, timeOptions, locationOptions, members);
    }

    [Fact]
    public async Task Handle_ByOrganizer_ConfirmsEventWithChosenOptions()
    {
        var organizerId = Guid.CreateVersion7();
        var (@event, timeOption, locationOption, events, timeOptions, locationOptions, members) = Setup(organizerId);

        var handler = new ConfirmEventCommandHandler(
            events, timeOptions, locationOptions, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(organizerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.Handle(new ConfirmEventCommand(@event.Id, timeOption.Id, locationOption.Id), CancellationToken.None);

        Assert.Equal(EventStatus.Confirmed, result.Status);
        Assert.Equal(timeOption.Id, result.ConfirmedTimeOptionId);
        Assert.Equal(locationOption.Id, result.ConfirmedLocationOptionId);
    }

    [Fact]
    public async Task Handle_ByNonOrganizerMember_ThrowsForbiddenException()
    {
        var organizerId = Guid.CreateVersion7();
        var otherMemberId = Guid.CreateVersion7();
        var (@event, timeOption, locationOption, events, timeOptions, locationOptions, members) = Setup(organizerId);
        members.Members.Add(new GroupMember { GroupId = @event.GroupId, UserId = otherMemberId, Role = GroupRole.Member });

        var handler = new ConfirmEventCommandHandler(
            events, timeOptions, locationOptions, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(otherMemberId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ForbiddenException>(
            () => handler.Handle(new ConfirmEventCommand(@event.Id, timeOption.Id, locationOption.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithOptionFromDifferentEvent_ThrowsNotFoundException()
    {
        var organizerId = Guid.CreateVersion7();
        var (@event, _, locationOption, events, timeOptions, locationOptions, members) = Setup(organizerId);

        var foreignTimeOption = new EventTimeOption { EventId = Guid.CreateVersion7(), StartUtc = DateTimeOffset.UtcNow, EndUtc = DateTimeOffset.UtcNow.AddHours(1), ProposedByUserId = organizerId };
        timeOptions.Options.Add(foreignTimeOption);

        var handler = new ConfirmEventCommandHandler(
            events, timeOptions, locationOptions, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(organizerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new ConfirmEventCommand(@event.Id, foreignTimeOption.Id, locationOption.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OnCancelledEvent_ThrowsConflictException()
    {
        var organizerId = Guid.CreateVersion7();
        var (@event, timeOption, locationOption, events, timeOptions, locationOptions, members) = Setup(organizerId);
        @event.Status = EventStatus.Cancelled;

        var handler = new ConfirmEventCommandHandler(
            events, timeOptions, locationOptions, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(organizerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new ConfirmEventCommand(@event.Id, timeOption.Id, locationOption.Id), CancellationToken.None));
    }
}
