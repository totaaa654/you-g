import 'package:freezed_annotation/freezed_annotation.dart';

import 'friend_request_status.dart';
import 'public_profile.dart';

part 'friend_request.freezed.dart';

@freezed
abstract class FriendRequest with _$FriendRequest {
  const factory FriendRequest({
    required String id,
    required PublicProfile profile,
    required FriendRequestStatus status,
    required DateTime createdAt,
  }) = _FriendRequest;
}
