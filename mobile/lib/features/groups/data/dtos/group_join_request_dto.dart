import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../friends/data/dtos/public_profile_dto.dart';

part 'group_join_request_dto.freezed.dart';
part 'group_join_request_dto.g.dart';

/// Mirrors backend `GroupJoinRequestDto` exactly.
@freezed
abstract class GroupJoinRequestDto with _$GroupJoinRequestDto {
  const factory GroupJoinRequestDto({
    required String id,
    required PublicProfileDto profile,
    required DateTime createdAt,
  }) = _GroupJoinRequestDto;

  factory GroupJoinRequestDto.fromJson(Map<String, dynamic> json) => _$GroupJoinRequestDtoFromJson(json);
}
