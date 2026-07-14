import 'package:freezed_annotation/freezed_annotation.dart';

import 'public_profile_dto.dart';

part 'friend_dto.freezed.dart';
part 'friend_dto.g.dart';

/// Mirrors backend `FriendDto` exactly.
@freezed
abstract class FriendDto with _$FriendDto {
  const factory FriendDto({
    required PublicProfileDto profile,
    required bool isFavorite,
    required DateTime friendedSince,
  }) = _FriendDto;

  factory FriendDto.fromJson(Map<String, dynamic> json) => _$FriendDtoFromJson(json);
}
