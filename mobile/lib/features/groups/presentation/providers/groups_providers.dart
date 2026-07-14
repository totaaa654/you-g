import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../data/datasources/groups_remote_data_source.dart';
import '../../data/repositories/groups_repository_impl.dart';
import '../../domain/entities/group.dart';
import '../../domain/entities/group_member.dart';
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

  Future<void> joinByInviteCode(String code) async {
    final group = await ref.read(groupsRepositoryProvider).joinByInviteCode(code);
    state = state.whenData((groups) => [group, ...groups.where((g) => g.id != group.id)]);
  }

  Future<void> leaveGroup(String groupId) async {
    await ref.read(groupsRepositoryProvider).leaveGroup(groupId);
    state = state.whenData((groups) => groups.where((g) => g.id != groupId).toList());
  }
}
