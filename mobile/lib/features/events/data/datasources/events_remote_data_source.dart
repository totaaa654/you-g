import 'package:dio/dio.dart';

import '../dtos/event_attendance_dto.dart';
import '../dtos/event_detail_dto.dart';
import '../dtos/event_dto.dart';
import '../dtos/event_location_option_dto.dart';
import '../dtos/event_time_option_dto.dart';

class EventsRemoteDataSource {
  EventsRemoteDataSource(this._dio);

  final Dio _dio;

  Future<EventDto> createEvent({
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
    final response = await _dio.post('/groups/$groupId/events', data: {
      'title': title,
      'description': description,
      'maxAttendees': maxAttendees,
      'initialStartUtc': initialStartUtc.toUtc().toIso8601String(),
      'initialEndUtc': initialEndUtc.toUtc().toIso8601String(),
      'initialLocationName': initialLocationName,
      'initialLocationAddress': initialLocationAddress,
      'initialLocationLatitude': initialLocationLatitude,
      'initialLocationLongitude': initialLocationLongitude,
    });
    return EventDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<List<EventDto>> getGroupEvents(String groupId) async {
    final response = await _dio.get('/groups/$groupId/events');
    return (response.data as List).map((e) => EventDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<EventDetailDto> getEventById(String id) async {
    final response = await _dio.get('/events/$id');
    return EventDetailDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<EventDto> updateEvent(String id, {required String title, String? description, int? maxAttendees}) async {
    final response = await _dio.patch(
      '/events/$id',
      data: {'title': title, 'description': description, 'maxAttendees': maxAttendees},
    );
    return EventDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> cancelEvent(String id) => _dio.delete('/events/$id');

  Future<EventTimeOptionDto> proposeTimeOption(
    String eventId, {
    required DateTime startUtc,
    required DateTime endUtc,
  }) async {
    final response = await _dio.post('/events/$eventId/time-options', data: {
      'startUtc': startUtc.toUtc().toIso8601String(),
      'endUtc': endUtc.toUtc().toIso8601String(),
    });
    return EventTimeOptionDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> voteTimeOption(String eventId, String optionId) =>
      _dio.put('/events/$eventId/time-options/$optionId/vote');

  Future<void> retractTimeVote(String eventId, String optionId) =>
      _dio.delete('/events/$eventId/time-options/$optionId/vote');

  Future<EventLocationOptionDto> proposeLocationOption(
    String eventId, {
    required String name,
    String? address,
    required double latitude,
    required double longitude,
  }) async {
    final response = await _dio.post('/events/$eventId/location-options', data: {
      'name': name,
      'address': address,
      'latitude': latitude,
      'longitude': longitude,
    });
    return EventLocationOptionDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> voteLocationOption(String eventId, String optionId) =>
      _dio.put('/events/$eventId/location-options/$optionId/vote');

  Future<void> retractLocationVote(String eventId, String optionId) =>
      _dio.delete('/events/$eventId/location-options/$optionId/vote');

  Future<EventDto> confirmEvent(String eventId, {required String timeOptionId, required String locationOptionId}) async {
    final response = await _dio.post(
      '/events/$eventId/confirm',
      data: {'timeOptionId': timeOptionId, 'locationOptionId': locationOptionId},
    );
    return EventDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<EventAttendanceDto> setAttendance(String eventId, String status) async {
    final response = await _dio.put('/events/$eventId/attendance', data: {'status': status});
    return EventAttendanceDto.fromJson(response.data as Map<String, dynamic>);
  }
}
