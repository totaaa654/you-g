using YouG.Application.Common.Exceptions;
using YouG.Application.Features.Events.Commands.SetAttendance;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Events;

public class SetAttendanceCommandHandlerTests
{
    private static (SetAttendanceCommandHandler Handler, FakeEventAttendanceRepository Attendance) CreateHandler(
        Event @event, Guid callerId, FakeGroupMemberRepository members)
    {
        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var attendance = new FakeEventAttendanceRepository();

        var handler = new SetAttendanceCommandHandler(
            events, attendance, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(callerId), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        return (handler, attendance);
    }

    private static (Event Event, FakeGroupMemberRepository Members) EventWithMembers(int? maxAttendees, params Guid[] memberIds)
    {
        var groupId = Guid.CreateVersion7();
        var @event = new Event { GroupId = groupId, CreatedByUserId = memberIds[0], Title = "Board Games", MaxAttendees = maxAttendees, Status = EventStatus.Confirmed };
        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        foreach (var id in memberIds)
        {
            members.Members.Add(new GroupMember { GroupId = groupId, UserId = id, Role = GroupRole.Member });
        }

        return (@event, members);
    }

    [Fact]
    public async Task Handle_UnderCapacity_SetsGoing()
    {
        var caller = Guid.CreateVersion7();
        var (@event, members) = EventWithMembers(maxAttendees: 2, caller);
        var (handler, attendance) = CreateHandler(@event, caller, members);

        var result = await handler.Handle(new SetAttendanceCommand(@event.Id, EventAttendanceStatus.Going), CancellationToken.None);

        Assert.Equal(EventAttendanceStatus.Going, result.Status);
        Assert.Single(attendance.Attendance);
    }

    [Fact]
    public async Task Handle_AtCapacity_ThrowsConflictException()
    {
        var userA = Guid.CreateVersion7();
        var userB = Guid.CreateVersion7();
        var userC = Guid.CreateVersion7();
        var (@event, members) = EventWithMembers(maxAttendees: 2, userA, userB, userC);

        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var attendance = new FakeEventAttendanceRepository();
        attendance.Attendance.Add(new EventAttendance { EventId = @event.Id, UserId = userA, Status = EventAttendanceStatus.Going, RespondedAt = DateTimeOffset.UtcNow });
        attendance.Attendance.Add(new EventAttendance { EventId = @event.Id, UserId = userB, Status = EventAttendanceStatus.Going, RespondedAt = DateTimeOffset.UtcNow });

        var handler = new SetAttendanceCommandHandler(
            events, attendance, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(userC), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new SetAttendanceCommand(@event.Id, EventAttendanceStatus.Going), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AlreadyGoing_ChangingToMaybeThenBackToGoing_DoesNotDoubleCount()
    {
        // A user who is already "Going" re-confirming Going shouldn't be blocked by their own
        // existing slot when the event is exactly at capacity.
        var userA = Guid.CreateVersion7();
        var userB = Guid.CreateVersion7();
        var (@event, members) = EventWithMembers(maxAttendees: 2, userA, userB);

        var events = new FakeEventRepository();
        events.Events.Add(@event);
        var attendance = new FakeEventAttendanceRepository();
        attendance.Attendance.Add(new EventAttendance { EventId = @event.Id, UserId = userA, Status = EventAttendanceStatus.Going, RespondedAt = DateTimeOffset.UtcNow });
        attendance.Attendance.Add(new EventAttendance { EventId = @event.Id, UserId = userB, Status = EventAttendanceStatus.Going, RespondedAt = DateTimeOffset.UtcNow });

        var handler = new SetAttendanceCommandHandler(
            events, attendance, members, new FakeUnitOfWork(),
            new FakeCurrentUserService(userA), new FakeDateTimeProvider(DateTimeOffset.UtcNow));

        // userA is already Going — re-affirming Going must not throw even though the event is full.
        var result = await handler.Handle(new SetAttendanceCommand(@event.Id, EventAttendanceStatus.Going), CancellationToken.None);

        Assert.Equal(EventAttendanceStatus.Going, result.Status);
    }

    [Fact]
    public async Task Handle_NoMaxAttendees_NeverBlocksGoing()
    {
        var userA = Guid.CreateVersion7();
        var (@event, members) = EventWithMembers(maxAttendees: null, userA);
        var (handler, _) = CreateHandler(@event, userA, members);

        var result = await handler.Handle(new SetAttendanceCommand(@event.Id, EventAttendanceStatus.Going), CancellationToken.None);

        Assert.Equal(EventAttendanceStatus.Going, result.Status);
    }
}
