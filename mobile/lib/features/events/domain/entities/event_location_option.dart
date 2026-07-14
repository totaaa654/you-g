import 'package:freezed_annotation/freezed_annotation.dart';

part 'event_location_option.freezed.dart';

@freezed
abstract class EventLocationOption with _$EventLocationOption {
  const factory EventLocationOption({
    required String id,
    required String name,
    String? address,
    required double latitude,
    required double longitude,
    required String proposedByUserId,
    required int voteCount,
    required bool hasCurrentUserVoted,
  }) = _EventLocationOption;
}
