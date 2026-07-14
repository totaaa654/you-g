import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../data/datasources/friends_remote_data_source.dart';
import '../../data/repositories/friends_repository_impl.dart';
import '../../domain/entities/friend.dart';
import '../../domain/entities/friend_request.dart';
import '../../domain/entities/friend_request_status.dart';
import '../../domain/entities/public_profile.dart';
import '../../domain/repositories/friends_repository.dart';

final friendsRemoteDataSourceProvider =
    Provider<FriendsRemoteDataSource>((ref) => FriendsRemoteDataSource(ref.watch(dioProvider)));

final friendsRepositoryProvider =
    Provider<FriendsRepository>((ref) => FriendsRepositoryImpl(ref.watch(friendsRemoteDataSourceProvider)));

final friendsListProvider =
    AsyncNotifierProvider<FriendsListNotifier, List<Friend>>(FriendsListNotifier.new);

final incomingFriendRequestsProvider = AsyncNotifierProvider<IncomingFriendRequestsNotifier, List<FriendRequest>>(
  IncomingFriendRequestsNotifier.new,
);

final outgoingFriendRequestsProvider = FutureProvider.autoDispose<List<FriendRequest>>((ref) {
  ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
  return ref.watch(friendsRepositoryProvider).getFriendRequests(FriendRequestDirection.outgoing);
});

/// Owns the friends list plus the mutations that change it (favorite/remove), so every
/// screen reading `friendsListProvider` sees the update immediately instead of needing a
/// manual refetch.
class FriendsListNotifier extends AsyncNotifier<List<Friend>> {
  @override
  Future<List<Friend>> build() {
    // Re-runs on login/logout/account switch — see the identical comment in
    // `MyGroupsNotifier.build()` for why this is necessary.
    ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
    return ref.watch(friendsRepositoryProvider).getFriends();
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => ref.read(friendsRepositoryProvider).getFriends());
  }

  Future<void> setFavorite(String userId, bool isFavorite) async {
    final repository = ref.read(friendsRepositoryProvider);
    await repository.setFavorite(userId, isFavorite);

    state = state.whenData(
      (friends) => [
        for (final friend in friends)
          if (friend.profile.id == userId) friend.copyWith(isFavorite: isFavorite) else friend,
      ],
    );
  }

  Future<void> removeFriend(String userId) async {
    final repository = ref.read(friendsRepositoryProvider);
    await repository.removeFriend(userId);
    state = state.whenData((friends) => friends.where((f) => f.profile.id != userId).toList());
  }
}

/// Separate from outgoing requests (`FutureProvider`, read-only) because incoming requests
/// support in-place mutation (accept/decline) that should update the badge count instantly.
class IncomingFriendRequestsNotifier extends AsyncNotifier<List<FriendRequest>> {
  @override
  Future<List<FriendRequest>> build() {
    ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
    return ref.watch(friendsRepositoryProvider).getFriendRequests(FriendRequestDirection.incoming);
  }

  Future<void> respond(String requestId, FriendRequestStatus status) async {
    final repository = ref.read(friendsRepositoryProvider);
    await repository.respondToFriendRequest(requestId, status);
    state = state.whenData((requests) => requests.where((r) => r.id != requestId).toList());

    if (status == FriendRequestStatus.accepted) {
      await ref.read(friendsListProvider.notifier).refresh();
    }
  }
}

/// One-shot search — a plain function provider rather than state, since results are
/// transient (query -> results, no local mutation).
final userSearchProvider =
    FutureProvider.family.autoDispose<List<PublicProfile>, String>((ref, query) {
  if (query.trim().isEmpty) return Future.value(const []);
  return ref.watch(friendsRepositoryProvider).searchUsers(query.trim());
});
