import 'package:freezed_annotation/freezed_annotation.dart';

import 'public_profile_dto.dart';

part 'friend_request_dto.freezed.dart';
part 'friend_request_dto.g.dart';

/// Mirrors backend `FriendRequestDto` exactly. `status` stays a raw wire string here
/// ("Pending"/"Accepted"/"Declined") — the repository maps it to `FriendRequestStatus`.
@freezed
abstract class FriendRequestDto with _$FriendRequestDto {
  const factory FriendRequestDto({
    required String id,
    required PublicProfileDto profile,
    required String status,
    required DateTime createdAt,
  }) = _FriendRequestDto;

  factory FriendRequestDto.fromJson(Map<String, dynamic> json) => _$FriendRequestDtoFromJson(json);
}
