import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../data/datasources/groups_remote_data_source.dart';
import '../../data/repositories/groups_repository_impl.dart';
import '../../domain/entities/group.dart';
import '../../domain/entities/group_join_request.dart';
import '../../domain/entities/group_join_request_response.dart';
import '../../domain/entities/group_member.dart';
import '../../domain/entities/join_group_result.dart';
import '../../domain/repositories/groups_repository.dart';

final groupsRemoteDataSourceProvider =
    Provider<GroupsRemoteDataSource>((ref) => GroupsRemoteDataSource(ref.watch(dioProvider)));

final groupsRepositoryProvider =
    Provider<GroupsRepository>((ref) => GroupsRepositoryImpl(ref.watch(groupsRemoteDataSourceProvider)));

final myGroupsProvider = AsyncNotifierProvider<MyGroupsNotifier, List<Group>>(MyGroupsNotifier.new);

final groupMembersProvider =
    FutureProvider.family.autoDispose<List<GroupMember>, String>(
  (ref, groupId) => ref.watch(groupsRepositoryProvider).getMembers(groupId),
);

final groupByIdProvider = FutureProvider.family.autoDispose<Group, String>(
  (ref, groupId) => ref.watch(groupsRepositoryProvider).getGroupById(groupId),
);

// autoDispose so leaving Group Detail and coming back refetches — without it, a request
// created after the admin's first visit to this screen would never show up until a full app
// restart, since a plain (non-autoDispose) family provider's cache outlives the screen.
final groupJoinRequestsProvider =
    AsyncNotifierProvider.autoDispose.family<GroupJoinRequestsNotifier, List<GroupJoinRequest>, String>(
  GroupJoinRequestsNotifier.new,
);

class GroupJoinRequestsNotifier extends AutoDisposeFamilyAsyncNotifier<List<GroupJoinRequest>, String> {
  @override
  Future<List<GroupJoinRequest>> build(String groupId) =>
      ref.watch(groupsRepositoryProvider).getJoinRequests(groupId);

  Future<void> respond(String groupId, String requestId, GroupJoinRequestResponse response) async {
    await ref.read(groupsRepositoryProvider).respondToJoinRequest(groupId, requestId, response);
    state = state.whenData((requests) => requests.where((r) => r.id != requestId).toList());
    if (response == GroupJoinRequestResponse.accepted) {
      ref.invalidate(groupMembersProvider(groupId));
      // Accepting changes the group's member count, which `myGroupsProvider` caches (shown on
      // the Groups tab's tiles) — that cache has no other reason to know this happened.
      await ref.read(myGroupsProvider.notifier).refresh();
    }
  }
}

class MyGroupsNotifier extends AsyncNotifier<List<Group>> {
  @override
  Future<List<Group>> build() {
    // Re-runs whenever the logged-in user changes (login/logout/account switch) — otherwise
    // this provider's cache would outlive the session and leak the previous account's groups,
    // since the bottom-nav shell keeps every tab's widget tree alive in the background.
    ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
    return ref.watch(groupsRepositoryProvider).getMyGroups();
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => ref.read(groupsRepositoryProvider).getMyGroups());
  }

  Future<Group> createGroup({required String name, String? description}) async {
    final group = await ref.read(groupsRepositoryProvider).createGroup(name: name, description: description);
    state = state.whenData((groups) => [group, ...groups]);
    return group;
  }

  /// Returns the result so the caller can tell the user whether they joined instantly or a
  /// join request is now pending admin approval (see `JoinGroupResult`).
  Future<JoinGroupResult> joinByInviteCode(String code) async {
    final result = await ref.read(groupsRepositoryProvider).joinByInviteCode(code);
    final group = result.group;
    if (group != null) {
      state = state.whenData((groups) => [group, ...groups.where((g) => g.id != group.id)]);
    }
    return result;
  }

  Future<void> leaveGroup(String groupId) async {
    await ref.read(groupsRepositoryProvider).leaveGroup(groupId);
    state = state.whenData((groups) => groups.where((g) => g.id != groupId).toList());
  }
}
