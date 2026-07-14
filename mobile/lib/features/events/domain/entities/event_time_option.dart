import 'package:freezed_annotation/freezed_annotation.dart';

part 'event_time_option.freezed.dart';

@freezed
abstract class EventTimeOption with _$EventTimeOption {
  const factory EventTimeOption({
    required String id,
    required DateTime startUtc,
    required DateTime endUtc,
    required String proposedByUserId,
    required int voteCount,
    required bool hasCurrentUserVoted,
  }) = _EventTimeOption;
}
