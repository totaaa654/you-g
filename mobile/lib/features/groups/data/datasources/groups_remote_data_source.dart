import 'package:dio/dio.dart';

import '../dtos/group_dto.dart';
import '../dtos/group_member_dto.dart';
import '../dtos/invite_link_dto.dart';

class GroupsRemoteDataSource {
  GroupsRemoteDataSource(this._dio);

  final Dio _dio;

  Future<List<GroupDto>> getMyGroups() async {
    final response = await _dio.get('/groups');
    return (response.data as List).map((e) => GroupDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<GroupDto> getGroupById(String id) async {
    final response = await _dio.get('/groups/$id');
    return GroupDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<GroupDto> createGroup({required String name, String? description}) async {
    final response = await _dio.post('/groups', data: {'name': name, 'description': description});
    return GroupDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<GroupDto> updateGroup(String id, {required String name, String? description}) async {
    final response = await _dio.patch('/groups/$id', data: {'name': name, 'description': description});
    return GroupDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> leaveGroup(String id) => _dio.delete('/groups/$id/members/me');

  Future<List<GroupMemberDto>> getMembers(String groupId) async {
    final response = await _dio.get('/groups/$groupId/members');
    return (response.data as List).map((e) => GroupMemberDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<void> removeMember(String groupId, String userId) => _dio.delete('/groups/$groupId/members/$userId');

  Future<void> changeMemberRole(String groupId, String userId, String role) =>
      _dio.patch('/groups/$groupId/members/$userId', data: {'role': role});

  Future<InviteLinkDto> createInviteLink(String groupId) async {
    final response = await _dio.post('/groups/$groupId/invite-links');
    return InviteLinkDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<GroupDto> joinByInviteCode(String code) async {
    final response = await _dio.post('/groups/join/$code');
    return GroupDto.fromJson(response.data as Map<String, dynamic>);
  }
}
