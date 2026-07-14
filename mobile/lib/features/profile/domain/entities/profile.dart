import 'package:freezed_annotation/freezed_annotation.dart';

import 'settings.dart';

part 'profile.freezed.dart';

@freezed
abstract class Profile with _$Profile {
  const factory Profile({
    required String id,
    required String email,
    required String username,
    required String displayName,
    String? bio,
    String? profilePictureUrl,
    required String timeZoneId,
    required String friendCode,
    required DateTime createdAt,
    required Settings settings,
  }) = _Profile;
}
