import 'package:freezed_annotation/freezed_annotation.dart';

import 'public_profile_dto.dart';

part 'search_users_result_dto.freezed.dart';
part 'search_users_result_dto.g.dart';

/// Mirrors backend `SearchUsersResultDto` exactly.
@freezed
abstract class SearchUsersResultDto with _$SearchUsersResultDto {
  const factory SearchUsersResultDto({
    required List<PublicProfileDto> users,
    required int page,
    required int pageSize,
    required int totalCount,
  }) = _SearchUsersResultDto;

  factory SearchUsersResultDto.fromJson(Map<String, dynamic> json) => _$SearchUsersResultDtoFromJson(json);
}
