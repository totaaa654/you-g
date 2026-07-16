import 'package:freezed_annotation/freezed_annotation.dart';

import 'group_dto.dart';

part 'join_group_result_dto.freezed.dart';
part 'join_group_result_dto.g.dart';

/// Mirrors backend `JoinGroupResultDto` exactly. `group` is null when `joined` is false —
/// the invite led to a pending `GroupJoinRequest` instead of instant membership.
@freezed
abstract class JoinGroupResultDto with _$JoinGroupResultDto {
  const factory JoinGroupResultDto({required bool joined, GroupDto? group}) = _JoinGroupResultDto;

  factory JoinGroupResultDto.fromJson(Map<String, dynamic> json) => _$JoinGroupResultDtoFromJson(json);
}
