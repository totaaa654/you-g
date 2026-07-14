import '../entities/event.dart';
import '../entities/event_attendance.dart';
import '../entities/event_detail.dart';
import '../entities/event_location_option.dart';
import '../entities/event_status.dart';
import '../entities/event_time_option.dart';

abstract class EventsRepository {
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
  });

  Future<List<Event>> getGroupEvents(String groupId);

  Future<EventDetail> getEventById(String id);

  Future<Event> updateEvent(String id, {required String title, String? description, int? maxAttendees});

  Future<void> cancelEvent(String id);

  Future<EventTimeOption> proposeTimeOption(String eventId, {required DateTime startUtc, required DateTime endUtc});

  Future<void> voteTimeOption(String eventId, String optionId);

  Future<void> retractTimeVote(String eventId, String optionId);

  Future<EventLocationOption> proposeLocationOption(
    String eventId, {
    required String name,
    String? address,
    required double latitude,
    required double longitude,
  });

  Future<void> voteLocationOption(String eventId, String optionId);

  Future<void> retractLocationVote(String eventId, String optionId);

  Future<Event> confirmEvent(String eventId, {required String timeOptionId, required String locationOptionId});

  Future<EventAttendance> setAttendance(String eventId, EventAttendanceStatus status);
}
