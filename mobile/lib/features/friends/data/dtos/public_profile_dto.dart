import 'package:freezed_annotation/freezed_annotation.dart';

part 'public_profile_dto.freezed.dart';
part 'public_profile_dto.g.dart';

/// Mirrors backend `PublicProfileDto` exactly (docs/04-API-DESIGN.md Section 3).
@freezed
abstract class PublicProfileDto with _$PublicProfileDto {
  const factory PublicProfileDto({
    required String id,
    required String username,
    required String displayName,
    String? bio,
    String? profilePictureUrl,
    required String friendCode,
  }) = _PublicProfileDto;

  factory PublicProfileDto.fromJson(Map<String, dynamic> json) => _$PublicProfileDtoFromJson(json);
}
