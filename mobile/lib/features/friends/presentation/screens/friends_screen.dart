import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/widgets/app_search_bar.dart';
import '../../../../core/widgets/empty_state.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../domain/entities/friend_request_status.dart';
import '../providers/friends_providers.dart';
import '../widgets/add_friend_sheet.dart';
import '../widgets/friend_request_tile.dart';
import '../widgets/friend_tile.dart';

class FriendsScreen extends ConsumerStatefulWidget {
  const FriendsScreen({super.key});

  @override
  ConsumerState<FriendsScreen> createState() => _FriendsScreenState();
}

class _FriendsScreenState extends ConsumerState<FriendsScreen> {
  String _query = '';

  void _openAddFriendSheet() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
      builder: (context) => const AddFriendSheet(),
    ).then((_) => ref.invalidate(incomingFriendRequestsProvider));
  }

  @override
  Widget build(BuildContext context) {
    final friendsAsync = ref.watch(friendsListProvider);
    final requestsAsync = ref.watch(incomingFriendRequestsProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Friends'),
        actions: [
          IconButton(onPressed: _openAddFriendSheet, icon: const Icon(Icons.person_add_alt_1_rounded)),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          await ref.read(friendsListProvider.notifier).refresh();
          ref.invalidate(incomingFriendRequestsProvider);
        },
        child: friendsAsync.when(
          loading: () => ListView(
            padding: const EdgeInsets.all(16),
            children: const [
              LoadingSkeleton(height: 64, borderRadius: 20),
              SizedBox(height: 12),
              LoadingSkeleton(height: 64, borderRadius: 20),
              SizedBox(height: 12),
              LoadingSkeleton(height: 64, borderRadius: 20),
            ],
          ),
          error: (error, _) => ListView(
            children: [
              EmptyState(
                icon: Icons.error_outline_rounded,
                title: "Couldn't load your friends",
                message: 'Pull down to try again.',
              ),
            ],
          ),
          data: (friends) {
            final requests = requestsAsync.valueOrNull ?? const [];
            final query = _query.trim().toLowerCase();
            final filtered = query.isEmpty
                ? friends
                : friends
                    .where((f) =>
                        f.profile.displayName.toLowerCase().contains(query) ||
                        f.profile.username.toLowerCase().contains(query))
                    .toList();
            final favorites = filtered.where((f) => f.isFavorite).toList();
            final everyoneElse = filtered.where((f) => !f.isFavorite).toList();

            if (friends.isEmpty && requests.isEmpty) {
              return ListView(
                children: [
                  EmptyState(
                    icon: Icons.people_outline_rounded,
                    title: 'No friends yet',
                    message: 'Search by username or share your friend code to get started.',
                    actionLabel: 'Add a friend',
                    onAction: _openAddFriendSheet,
                  ),
                ],
              );
            }

            return ListView(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
              children: [
                AppSearchBar(hintText: 'Search friends', onChanged: (v) => setState(() => _query = v)),
                const SizedBox(height: 20),
                if (requests.isNotEmpty) ...[
                  Text('Friend requests', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 10),
                  for (final request in requests) ...[
                    FriendRequestTile(
                      request: request,
                      onAccept: () => ref
                          .read(incomingFriendRequestsProvider.notifier)
                          .respond(request.id, FriendRequestStatus.accepted),
                      onDecline: () => ref
                          .read(incomingFriendRequestsProvider.notifier)
                          .respond(request.id, FriendRequestStatus.declined),
                    ),
                    const SizedBox(height: 10),
                  ],
                  const SizedBox(height: 10),
                ],
                if (favorites.isNotEmpty) ...[
                  Text('Favorites', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 10),
                  for (final friend in favorites) ...[
                    FriendTile(
                      friend: friend,
                      onToggleFavorite: (value) =>
                          ref.read(friendsListProvider.notifier).setFavorite(friend.profile.id, value),
                    ),
                    const SizedBox(height: 10),
                  ],
                  const SizedBox(height: 10),
                ],
                Text('All friends', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 10),
                if (everyoneElse.isEmpty && favorites.isEmpty)
                  const Padding(
                    padding: EdgeInsets.symmetric(vertical: 24),
                    child: Center(child: Text('No friends match your search.')),
                  )
                else
                  for (final friend in everyoneElse) ...[
                    FriendTile(
                      friend: friend,
                      onToggleFavorite: (value) =>
                          ref.read(friendsListProvider.notifier).setFavorite(friend.profile.id, value),
                    ),
                    const SizedBox(height: 10),
                  ],
              ],
            );
          },
        ),
      ),
    );
  }
}
