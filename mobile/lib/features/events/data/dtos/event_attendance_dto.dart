import 'package:freezed_annotation/freezed_annotation.dart';

part 'event_attendance_dto.freezed.dart';
part 'event_attendance_dto.g.dart';

/// Mirrors backend `EventAttendanceDto`. `status` stays a raw wire string.
@freezed
abstract class EventAttendanceDto with _$EventAttendanceDto {
  const factory EventAttendanceDto({
    required String userId,
    required String status,
    required DateTime respondedAt,
  }) = _EventAttendanceDto;

  factory EventAttendanceDto.fromJson(Map<String, dynamic> json) => _$EventAttendanceDtoFromJson(json);
}
