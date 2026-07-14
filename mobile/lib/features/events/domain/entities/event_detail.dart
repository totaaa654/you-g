import 'package:freezed_annotation/freezed_annotation.dart';

import 'event.dart';
import 'event_attendance.dart';
import 'event_location_option.dart';
import 'event_time_option.dart';

part 'event_detail.freezed.dart';

@freezed
abstract class EventDetail with _$EventDetail {
  const factory EventDetail({
    required Event event,
    required List<EventTimeOption> timeOptions,
    required List<EventLocationOption> locationOptions,
    required List<EventAttendance> attendance,
  }) = _EventDetail;
}
