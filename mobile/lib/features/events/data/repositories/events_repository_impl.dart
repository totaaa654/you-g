import '../../domain/entities/event.dart';
import '../../domain/entities/event_attendance.dart';
import '../../domain/entities/event_detail.dart';
import '../../domain/entities/event_location_option.dart';
import '../../domain/entities/event_status.dart';
import '../../domain/entities/event_time_option.dart';
import '../../domain/repositories/events_repository.dart';
import '../datasources/events_remote_data_source.dart';
import '../dtos/event_attendance_dto.dart';
import '../dtos/event_dto.dart';
import '../dtos/event_location_option_dto.dart';
import '../dtos/event_time_option_dto.dart';

class EventsRepositoryImpl implements EventsRepository {
  EventsRepositoryImpl(this._remoteDataSource);

  final EventsRemoteDataSource _remoteDataSource;

  @override
  Future<Event> createEvent({
    required String groupId,
    required String title,
    String? description,
    int? maxAttendees,
    required DateTime initialStartUtc,
    required DateTime initialEndUtc,
    String? initialLocationName,
    String? initialLocationAddress,
    double? initialLocationLatitude,
    double? initialLocationLongitude,
  }) async {
    final dto = await _remoteDataSource.createEvent(
      groupId: groupId,
      title: title,
      description: description,
      maxAttendees: maxAttendees,
      initialStartUtc: initialStartUtc,
      initialEndUtc: initialEndUtc,
      initialLocationName: initialLocationName,
      initialLocationAddress: initialLocationAddress,
      initialLocationLatitude: initialLocationLatitude,
      initialLocationLongitude: initialLocationLongitude,
    );
    return _mapEvent(dto);
  }

  @override
  Future<List<Event>> getGroupEvents(String groupId) async {
    final dtos = await _remoteDataSource.getGroupEvents(groupId);
    return dtos.map(_mapEvent).toList();
  }

  @override
  Future<EventDetail> getEventById(String id) async {
    final dto = await _remoteDataSource.getEventById(id);
    return EventDetail(
      event: _mapEvent(dto.event),
      timeOptions: dto.timeOptions.map(_mapTimeOption).toList(),
      locationOptions: dto.locationOptions.map(_mapLocationOption).toList(),
      attendance: dto.attendance.map(_mapAttendance).toList(),
    );
  }

  @override
  Future<Event> updateEvent(String id, {required String title, String? description, int? maxAttendees}) async =>
      _mapEvent(await _remoteDataSource.updateEvent(id, title: title, description: description, maxAttendees: maxAttendees));

  @override
  Future<void> cancelEvent(String id) => _remoteDataSource.cancelEvent(id);

  @override
  Future<EventTimeOption> proposeTimeOption(String eventId, {required DateTime startUtc, required DateTime endUtc}) async =>
      _mapTimeOption(await _remoteDataSource.proposeTimeOption(eventId, startUtc: startUtc, endUtc: endUtc));

  @override
  Future<void> voteTimeOption(String eventId, String optionId) => _remoteDataSource.voteTimeOption(eventId, optionId);

  @override
  Future<void> retractTimeVote(String eventId, String optionId) => _remoteDataSource.retractTimeVote(eventId, optionId);

  @override
  Future<EventLocationOption> proposeLocationOption(
    String eventId, {
    required String name,
    String? address,
    required double latitude,
    required double longitude,
  }) async =>
      _mapLocationOption(
        await _remoteDataSource.proposeLocationOption(eventId, name: name, address: address, latitude: latitude, longitude: longitude),
      );

  @override
  Future<void> voteLocationOption(String eventId, String optionId) =>
      _remoteDataSource.voteLocationOption(eventId, optionId);

  @override
  Future<void> retractLocationVote(String eventId, String optionId) =>
      _remoteDataSource.retractLocationVote(eventId, optionId);

  @override
  Future<Event> confirmEvent(String eventId, {required String timeOptionId, required String locationOptionId}) async =>
      _mapEvent(await _remoteDataSource.confirmEvent(eventId, timeOptionId: timeOptionId, locationOptionId: locationOptionId));

  @override
  Future<EventAttendance> setAttendance(String eventId, EventAttendanceStatus status) async =>
      _mapAttendance(await _remoteDataSource.setAttendance(eventId, status.toJson()));

  Event _mapEvent(EventDto dto) => Event(
        id: dto.id,
        groupId: dto.groupId,
        createdByUserId: dto.createdByUserId,
        title: dto.title,
        description: dto.description,
        maxAttendees: dto.maxAttendees,
        status: EventStatus.fromJson(dto.status),
        confirmedTimeOptionId: dto.confirmedTimeOptionId,
        confirmedLocationOptionId: dto.confirmedLocationOptionId,
        createdAt: dto.createdAt,
      );

  EventTimeOption _mapTimeOption(EventTimeOptionDto dto) => EventTimeOption(
        id: dto.id,
        startUtc: dto.startUtc,
        endUtc: dto.endUtc,
        proposedByUserId: dto.proposedByUserId,
        voteCount: dto.voteCount,
        hasCurrentUserVoted: dto.hasCurrentUserVoted,
      );

  EventLocationOption _mapLocationOption(EventLocationOptionDto dto) => EventLocationOption(
        id: dto.id,
        name: dto.name,
        address: dto.address,
        latitude: dto.latitude,
        longitude: dto.longitude,
        proposedByUserId: dto.proposedByUserId,
        voteCount: dto.voteCount,
        hasCurrentUserVoted: dto.hasCurrentUserVoted,
      );

  EventAttendance _mapAttendance(EventAttendanceDto dto) =>
      EventAttendance(userId: dto.userId, status: EventAttendanceStatus.fromJson(dto.status), respondedAt: dto.respondedAt);
}
