using YouG.Application.Features.Events.Queries.GetEventById;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Events;

public class GetEventByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_UserRemovedFromGroup_ExcludesTheirAttendanceAndVotes()
    {
        var now = DateTimeOffset.UtcNow;
        var groupId = Guid.CreateVersion7();
        var adminId = Guid.CreateVersion7();
        var removedUserId = Guid.CreateVersion7();

        var members = new FakeGroupMemberRepository(new FakeGroupRepository());
        // Only the admin is still a member — removedUserId used to be, but was kicked.
        members.Members.Add(new GroupMember { GroupId = groupId, UserId = adminId, Role = GroupRole.Admin, JoinedAt = now });

        var events = new FakeEventRepository();
        var @event = new Event
        {
            GroupId = groupId, CreatedByUserId = adminId, Title = "Game night", CreatedAt = now, UpdatedAt = now
        };
        events.Events.Add(@event);

        var timeOptions = new FakeEventTimeOptionRepository();
        var timeOption = new EventTimeOption
        {
            EventId = @event.Id, StartUtc = now, EndUtc = now.AddHours(2), ProposedByUserId = adminId, CreatedAt = now
        };
        timeOptions.Options.Add(timeOption);

        var timeVotes = new FakeEventTimeVoteRepository();
        timeVotes.Votes.Add(new EventTimeVote { EventTimeOptionId = timeOption.Id, UserId = adminId, CreatedAt = now });
        timeVotes.Votes.Add(new EventTimeVote { EventTimeOptionId = timeOption.Id, UserId = removedUserId, CreatedAt = now });

        var locationOptions = new FakeEventLocationOptionRepository();
        var locationOption = new EventLocationOption
        {
            EventId = @event.Id, Name = "Park", Latitude = 0, Longitude = 0, ProposedByUserId = adminId, CreatedAt = now
        };
        locationOptions.Options.Add(locationOption);

        var locationVotes = new FakeEventLocationVoteRepository();
        locationVotes.Votes.Add(new EventLocationVote { EventLocationOptionId = locationOption.Id, UserId = adminId, CreatedAt = now });
        locationVotes.Votes.Add(new EventLocationVote { EventLocationOptionId = locationOption.Id, UserId = removedUserId, CreatedAt = now });

        var attendance = new FakeEventAttendanceRepository();
        attendance.Attendance.Add(new EventAttendance { EventId = @event.Id, UserId = adminId, Status = EventAttendanceStatus.Going, RespondedAt = now });
        attendance.Attendance.Add(new EventAttendance { EventId = @event.Id, UserId = removedUserId, Status = EventAttendanceStatus.Going, RespondedAt = now });

        var handler = new GetEventByIdQueryHandler(
            events, timeOptions, locationOptions, timeVotes, locationVotes, attendance, members, new FakeCurrentUserService(adminId));

        var result = await handler.Handle(new GetEventByIdQuery(@event.Id), CancellationToken.None);

        Assert.Single(result.Attendance);
        Assert.Equal(adminId, result.Attendance[0].UserId);
        Assert.Equal(1, result.TimeOptions[0].VoteCount);
        Assert.Equal(1, result.LocationOptions[0].VoteCount);
    }
}
