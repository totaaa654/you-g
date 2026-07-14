import 'package:freezed_annotation/freezed_annotation.dart';

part 'public_profile.freezed.dart';

@freezed
abstract class PublicProfile with _$PublicProfile {
  const factory PublicProfile({
    required String id,
    required String username,
    required String displayName,
    String? bio,
    String? profilePictureUrl,
    required String friendCode,
  }) = _PublicProfile;
}
