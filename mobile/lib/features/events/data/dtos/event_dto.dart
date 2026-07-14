import 'package:freezed_annotation/freezed_annotation.dart';

part 'event_dto.freezed.dart';
part 'event_dto.g.dart';

/// Mirrors backend `EventDto` exactly. `status` stays a raw wire string.
@freezed
abstract class EventDto with _$EventDto {
  const factory EventDto({
    required String id,
    required String groupId,
    required String createdByUserId,
    required String title,
    String? description,
    int? maxAttendees,
    required String status,
    String? confirmedTimeOptionId,
    String? confirmedLocationOptionId,
    required DateTime createdAt,
  }) = _EventDto;

  factory EventDto.fromJson(Map<String, dynamic> json) => _$EventDtoFromJson(json);
}
