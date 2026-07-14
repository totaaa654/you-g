import 'package:freezed_annotation/freezed_annotation.dart';

part 'event_location_option_dto.freezed.dart';
part 'event_location_option_dto.g.dart';

@freezed
abstract class EventLocationOptionDto with _$EventLocationOptionDto {
  const factory EventLocationOptionDto({
    required String id,
    required String name,
    String? address,
    required double latitude,
    required double longitude,
    required String proposedByUserId,
    required int voteCount,
    required bool hasCurrentUserVoted,
  }) = _EventLocationOptionDto;

  factory EventLocationOptionDto.fromJson(Map<String, dynamic> json) => _$EventLocationOptionDtoFromJson(json);
}
