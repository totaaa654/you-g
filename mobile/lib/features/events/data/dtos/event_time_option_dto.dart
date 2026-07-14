import 'package:freezed_annotation/freezed_annotation.dart';

part 'event_time_option_dto.freezed.dart';
part 'event_time_option_dto.g.dart';

@freezed
abstract class EventTimeOptionDto with _$EventTimeOptionDto {
  const factory EventTimeOptionDto({
    required String id,
    required DateTime startUtc,
    required DateTime endUtc,
    required String proposedByUserId,
    required int voteCount,
    required bool hasCurrentUserVoted,
  }) = _EventTimeOptionDto;

  factory EventTimeOptionDto.fromJson(Map<String, dynamic> json) => _$EventTimeOptionDtoFromJson(json);
}
