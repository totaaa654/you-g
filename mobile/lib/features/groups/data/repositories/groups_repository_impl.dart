import '../../domain/entities/group.dart';
import '../../domain/entities/group_member.dart';
import '../../domain/entities/group_role.dart';
import '../../domain/entities/invite_link.dart';
import '../../domain/repositories/groups_repository.dart';
import '../datasources/groups_remote_data_source.dart';
import '../dtos/group_dto.dart';
import '../dtos/group_member_dto.dart';

class GroupsRepositoryImpl implements GroupsRepository {
  GroupsRepositoryImpl(this._remoteDataSource);

  final GroupsRemoteDataSource _remoteDataSource;

  @override
  Future<List<Group>> getMyGroups() async {
    final dtos = await _remoteDataSource.getMyGroups();
    return dtos.map(_mapGroup).toList();
  }

  @override
  Future<Group> getGroupById(String id) async => _mapGroup(await _remoteDataSource.getGroupById(id));

  @override
  Future<Group> createGroup({required String name, String? description}) async =>
      _mapGroup(await _remoteDataSource.createGroup(name: name, description: description));

  @override
  Future<Group> updateGroup(String id, {required String name, String? description}) async =>
      _mapGroup(await _remoteDataSource.updateGroup(id, name: name, description: description));

  @override
  Future<void> leaveGroup(String id) => _remoteDataSource.leaveGroup(id);

  @override
  Future<List<GroupMember>> getMembers(String groupId) async {
    final dtos = await _remoteDataSource.getMembers(groupId);
    return dtos.map(_mapMember).toList();
  }

  @override
  Future<void> removeMember(String groupId, String userId) => _remoteDataSource.removeMember(groupId, userId);

  @override
  Future<void> changeMemberRole(String groupId, String userId, GroupRole role) =>
      _remoteDataSource.changeMemberRole(groupId, userId, role.toJson());

  @override
  Future<InviteLink> createInviteLink(String groupId) async {
    final dto = await _remoteDataSource.createInviteLink(groupId);
    return InviteLink(code: dto.code, expiresAt: dto.expiresAt);
  }

  @override
  Future<Group> joinByInviteCode(String code) async => _mapGroup(await _remoteDataSource.joinByInviteCode(code));

  Group _mapGroup(GroupDto dto) => Group(
        id: dto.id,
        name: dto.name,
        description: dto.description,
        pictureUrl: dto.pictureUrl,
        createdByUserId: dto.createdByUserId,
        memberCount: dto.memberCount,
        createdAt: dto.createdAt,
      );

  GroupMember _mapMember(GroupMemberDto dto) => GroupMember(
        userId: dto.userId,
        username: dto.username,
        displayName: dto.displayName,
        profilePictureUrl: dto.profilePictureUrl,
        role: GroupRole.fromJson(dto.role),
        joinedAt: dto.joinedAt,
      );
}
