namespace YouG.Application.Features.Events.Dtos;

public record EventDetailDto(
    EventDto Event,
    List<EventTimeOptionDto> TimeOptions,
    List<EventLocationOptionDto> LocationOptions,
    List<EventAttendanceDto> Attendance);
