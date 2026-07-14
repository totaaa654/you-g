import 'package:freezed_annotation/freezed_annotation.dart';

import 'event_status.dart';

part 'event_attendance.freezed.dart';

@freezed
abstract class EventAttendance with _$EventAttendance {
  const factory EventAttendance({
    required String userId,
    required EventAttendanceStatus status,
    required DateTime respondedAt,
  }) = _EventAttendance;
}
