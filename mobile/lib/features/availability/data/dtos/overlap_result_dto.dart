import 'package:freezed_annotation/freezed_annotation.dart';

part 'overlap_result_dto.freezed.dart';
part 'overlap_result_dto.g.dart';

@freezed
abstract class OverlapWindowDto with _$OverlapWindowDto {
  const factory OverlapWindowDto({
    required DateTime date,
    required String startTime,
    required List<String> availableUserIds,
    required int availableCount,
    required int totalMembers,
    required List<String> maybeUserIds,
  }) = _OverlapWindowDto;

  factory OverlapWindowDto.fromJson(Map<String, dynamic> json) => _$OverlapWindowDtoFromJson(json);
}

/// Mirrors backend `OverlapResultDto` exactly.
@freezed
abstract class OverlapResultDto with _$OverlapResultDto {
  const factory OverlapResultDto({
    required String groupId,
    required List<OverlapWindowDto> windows,
  }) = _OverlapResultDto;

  factory OverlapResultDto.fromJson(Map<String, dynamic> json) => _$OverlapResultDtoFromJson(json);
}
