import 'package:freezed_annotation/freezed_annotation.dart';

import 'event_attendance_dto.dart';
import 'event_dto.dart';
import 'event_location_option_dto.dart';
import 'event_time_option_dto.dart';

part 'event_detail_dto.freezed.dart';
part 'event_detail_dto.g.dart';

/// Mirrors backend `EventDetailDto` exactly.
@freezed
abstract class EventDetailDto with _$EventDetailDto {
  const factory EventDetailDto({
    required EventDto event,
    required List<EventTimeOptionDto> timeOptions,
    required List<EventLocationOptionDto> locationOptions,
    required List<EventAttendanceDto> attendance,
  }) = _EventDetailDto;

  factory EventDetailDto.fromJson(Map<String, dynamic> json) => _$EventDetailDtoFromJson(json);
}
