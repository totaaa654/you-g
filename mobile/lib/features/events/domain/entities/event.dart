import 'package:freezed_annotation/freezed_annotation.dart';

import 'event_status.dart';

part 'event.freezed.dart';

@freezed
abstract class Event with _$Event {
  const factory Event({
    required String id,
    required String groupId,
    required String createdByUserId,
    required String title,
    String? description,
    int? maxAttendees,
    required EventStatus status,
    String? confirmedTimeOptionId,
    String? confirmedLocationOptionId,
    required DateTime createdAt,
  }) = _Event;
}
