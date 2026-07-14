import '../../domain/entities/friend.dart';
import '../../domain/entities/friend_request.dart';
import '../../domain/entities/friend_request_status.dart';
import '../../domain/entities/public_profile.dart';
import '../../domain/repositories/friends_repository.dart';
import '../datasources/friends_remote_data_source.dart';
import '../dtos/friend_dto.dart';
import '../dtos/friend_request_dto.dart';
import '../dtos/public_profile_dto.dart';

class FriendsRepositoryImpl implements FriendsRepository {
  FriendsRepositoryImpl(this._remoteDataSource);

  final FriendsRemoteDataSource _remoteDataSource;

  @override
  Future<List<Friend>> getFriends() async {
    final dtos = await _remoteDataSource.getFriends();
    return dtos.map(_mapFriend).toList();
  }

  @override
  Future<List<FriendRequest>> getFriendRequests(FriendRequestDirection direction) async {
    final dtos = await _remoteDataSource.getFriendRequests(direction.queryValue);
    return dtos.map(_mapFriendRequest).toList();
  }

  @override
  Future<FriendRequest> sendFriendRequest({String? addresseeId, String? friendCode}) async {
    final dto = await _remoteDataSource.sendFriendRequest(addresseeId: addresseeId, friendCode: friendCode);
    return _mapFriendRequest(dto);
  }

  @override
  Future<FriendRequest> respondToFriendRequest(String requestId, FriendRequestStatus status) async {
    final dto = await _remoteDataSource.respondToFriendRequest(requestId, status.toJson());
    return _mapFriendRequest(dto);
  }

  @override
  Future<void> removeFriend(String userId) => _remoteDataSource.removeFriend(userId);

  @override
  Future<void> setFavorite(String userId, bool isFavorite) => _remoteDataSource.setFavorite(userId, isFavorite);

  @override
  Future<List<PublicProfile>> searchUsers(String query) async {
    final result = await _remoteDataSource.searchUsers(query);
    return result.users.map(_mapProfile).toList();
  }

  @override
  Future<void> blockUser(String userId) => _remoteDataSource.blockUser(userId);

  PublicProfile _mapProfile(PublicProfileDto dto) => PublicProfile(
        id: dto.id,
        username: dto.username,
        displayName: dto.displayName,
        bio: dto.bio,
        profilePictureUrl: dto.profilePictureUrl,
        friendCode: dto.friendCode,
      );

  Friend _mapFriend(FriendDto dto) =>
      Friend(profile: _mapProfile(dto.profile), isFavorite: dto.isFavorite, friendedSince: dto.friendedSince);

  FriendRequest _mapFriendRequest(FriendRequestDto dto) => FriendRequest(
        id: dto.id,
        profile: _mapProfile(dto.profile),
        status: FriendRequestStatus.fromJson(dto.status),
        createdAt: dto.createdAt,
      );
}
