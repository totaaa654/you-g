import 'package:dio/dio.dart';

import '../dtos/friend_dto.dart';
import '../dtos/friend_request_dto.dart';
import '../dtos/search_users_result_dto.dart';

class FriendsRemoteDataSource {
  FriendsRemoteDataSource(this._dio);

  final Dio _dio;

  Future<List<FriendDto>> getFriends() async {
    final response = await _dio.get('/friends');
    return (response.data as List).map((e) => FriendDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<List<FriendRequestDto>> getFriendRequests(String direction) async {
    final response = await _dio.get('/friends/requests', queryParameters: {'direction': direction});
    return (response.data as List).map((e) => FriendRequestDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<FriendRequestDto> sendFriendRequest({String? addresseeId, String? friendCode}) async {
    final response = await _dio.post('/friends/requests', data: {
      if (addresseeId != null) 'addresseeId': addresseeId,
      if (friendCode != null) 'friendCode': friendCode,
    });
    return FriendRequestDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<FriendRequestDto> respondToFriendRequest(String requestId, String status) async {
    final response = await _dio.put('/friends/requests/$requestId', data: {'status': status});
    return FriendRequestDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> removeFriend(String userId) => _dio.delete('/friends/$userId');

  Future<void> setFavorite(String userId, bool isFavorite) =>
      _dio.patch('/friends/$userId', data: {'isFavorite': isFavorite});

  Future<SearchUsersResultDto> searchUsers(String query) async {
    final response = await _dio.get('/users/search', queryParameters: {'q': query, 'page': 1, 'pageSize': 20});
    return SearchUsersResultDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> blockUser(String userId) => _dio.post('/blocks', data: {'blockedUserId': userId});
}
