import 'package:freezed_annotation/freezed_annotation.dart';

import 'settings_dto.dart';

part 'profile_dto.freezed.dart';
part 'profile_dto.g.dart';

/// Mirrors backend `ProfileDto` exactly.
@freezed
abstract class ProfileDto with _$ProfileDto {
  const factory ProfileDto({
    required String id,
    required String email,
    required String username,
    required String displayName,
    String? bio,
    String? profilePictureUrl,
    required String timeZoneId,
    required String friendCode,
    required DateTime createdAt,
    required SettingsDto settings,
  }) = _ProfileDto;

  factory ProfileDto.fromJson(Map<String, dynamic> json) => _$ProfileDtoFromJson(json);
}
