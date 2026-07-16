import '../entities/group.dart';
import '../entities/group_join_request.dart';
import '../entities/group_join_request_response.dart';
import '../entities/group_member.dart';
import '../entities/group_role.dart';
import '../entities/invite_link.dart';
import '../entities/join_group_result.dart';

abstract class GroupsRepository {
  Future<List<Group>> getMyGroups();

  Future<Group> getGroupById(String id);

  Future<Group> createGroup({required String name, String? description});

  Future<Group> updateGroup(String id, {required String name, String? description});

  Future<void> leaveGroup(String id);

  Future<List<GroupMember>> getMembers(String groupId);

  Future<void> removeMember(String groupId, String userId);

  Future<void> changeMemberRole(String groupId, String userId, GroupRole role);

  Future<InviteLink> createInviteLink(String groupId);

  Future<JoinGroupResult> joinByInviteCode(String code);

  Future<List<GroupJoinRequest>> getJoinRequests(String groupId);

  Future<void> respondToJoinRequest(String groupId, String requestId, GroupJoinRequestResponse response);
}
