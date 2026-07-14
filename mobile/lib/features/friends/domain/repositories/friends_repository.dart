import '../entities/friend.dart';
import '../entities/friend_request.dart';
import '../entities/friend_request_status.dart';
import '../entities/public_profile.dart';

abstract class FriendsRepository {
  Future<List<Friend>> getFriends();

  Future<List<FriendRequest>> getFriendRequests(FriendRequestDirection direction);

  /// Exactly one of [addresseeId]/[friendCode] should be set — matches the backend's
  /// `{ addresseeId }` or `{ friendCode }` body contract.
  Future<FriendRequest> sendFriendRequest({String? addresseeId, String? friendCode});

  Future<FriendRequest> respondToFriendRequest(String requestId, FriendRequestStatus status);

  Future<void> removeFriend(String userId);

  Future<void> setFavorite(String userId, bool isFavorite);

  Future<List<PublicProfile>> searchUsers(String query);

  Future<void> blockUser(String userId);
}
