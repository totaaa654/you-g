using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Events.Commands.VoteTimeOption;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Events;

public class VoteTimeOptionCommandHandlerTests
{
    [Fact]
    public async Task Handle_FirstVote_AddsVote()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var @event = new Event { GroupId = groupId, CreatedByUserId = callerId, Title = "Board Games", Status = EventStatus.Proposed };
        var option = new EventTimeOption { EventId = @event.Id, StartUtc = DateTimeOffset.UtcNow, EndUtc = DateTimeOffset.UtcNow.AddHours(1), ProposedByUserId = callerId };

        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var timeOptions = new FakeEventTimeOptionRepository();
        timeOptions.Options.Add(option);
        var votes = new FakeEventTimeVoteRepository();
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = callerId, Role = GroupRole.Member });

        var handler = new VoteTimeOptionCommandHandler(
            events, timeOptions, votes, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(callerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await handler.Handle(new VoteTimeOptionCommand(@event.Id, option.Id), CancellationToken.None);

        Assert.Single(votes.Votes);
    }

    [Fact]
    public async Task Handle_VotingTwice_IsIdempotent()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var @event = new Event { GroupId = groupId, CreatedByUserId = callerId, Title = "Board Games", Status = EventStatus.Proposed };
        var option = new EventTimeOption { EventId = @event.Id, StartUtc = DateTimeOffset.UtcNow, EndUtc = DateTimeOffset.UtcNow.AddHours(1), ProposedByUserId = callerId };

        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var timeOptions = new FakeEventTimeOptionRepository();
        timeOptions.Options.Add(option);
        var votes = new FakeEventTimeVoteRepository();
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = callerId, Role = GroupRole.Member });

        var handler = new VoteTimeOptionCommandHandler(
            events, timeOptions, votes, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(callerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await handler.Handle(new VoteTimeOptionCommand(@event.Id, option.Id), CancellationToken.None);
        await handler.Handle(new VoteTimeOptionCommand(@event.Id, option.Id), CancellationToken.None);

        Assert.Single(votes.Votes); // still one vote, not two
    }

    [Fact]
    public async Task Handle_OptionFromDifferentEvent_ThrowsNotFoundException()
    {
        var groupId = Guid.CreateVersion7();
        var callerId = Guid.CreateVersion7();
        var @event = new Event { GroupId = groupId, CreatedByUserId = callerId, Title = "Board Games", Status = EventStatus.Proposed };
        var foreignOption = new EventTimeOption { EventId = Guid.CreateVersion7(), StartUtc = DateTimeOffset.UtcNow, EndUtc = DateTimeOffset.UtcNow.AddHours(1), ProposedByUserId = callerId };

        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var timeOptions = new FakeEventTimeOptionRepository();
        timeOptions.Options.Add(foreignOption);
        var votes = new FakeEventTimeVoteRepository();
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = callerId, Role = GroupRole.Member });

        var handler = new VoteTimeOptionCommandHandler(
            events, timeOptions, votes, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(callerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new VoteTimeOptionCommand(@event.Id, foreignOption.Id), CancellationToken.None));
    }
}
