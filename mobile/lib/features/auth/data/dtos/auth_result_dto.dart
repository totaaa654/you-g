import 'package:freezed_annotation/freezed_annotation.dart';

part 'auth_result_dto.freezed.dart';
part 'auth_result_dto.g.dart';

/// Mirrors backend AuthResultDto exactly (docs/04-API-DESIGN.md Section 3.1).
@freezed
abstract class AuthResultDto with _$AuthResultDto {
  const factory AuthResultDto({
    required String accessToken,
    required String refreshToken,
    required UserSummaryDto user,
  }) = _AuthResultDto;

  factory AuthResultDto.fromJson(Map<String, dynamic> json) => _$AuthResultDtoFromJson(json);
}

@freezed
abstract class UserSummaryDto with _$UserSummaryDto {
  const factory UserSummaryDto({
    required String id,
    required String username,
    required String displayName,
    required String friendCode,
  }) = _UserSummaryDto;

  factory UserSummaryDto.fromJson(Map<String, dynamic> json) => _$UserSummaryDtoFromJson(json);
}
