import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_card.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../../events/presentation/providers/events_providers.dart';
import '../../../events/presentation/widgets/event_tile.dart';
import '../../../friends/presentation/providers/friends_providers.dart';
import '../../domain/entities/group_join_request_response.dart';
import '../../domain/entities/group_member.dart';
import '../../domain/entities/group_role.dart';
import '../providers/groups_providers.dart';

/// Not part of bottom nav — the hub Smart Time Finder / Create Event / member management
/// are reached from.
class GroupDetailScreen extends ConsumerWidget {
  const GroupDetailScreen({required this.groupId, super.key});

  final String groupId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final groupAsync = ref.watch(groupByIdProvider(groupId));
    final membersAsync = ref.watch(groupMembersProvider(groupId));
    final eventsAsync = ref.watch(groupEventsProvider(groupId));
    final currentUserId = ref.watch(authControllerProvider).valueOrNull?.id;

    return Scaffold(
      appBar: AppBar(title: const Text('Group')),
      body: groupAsync.when(
        loading: () => ListView(
          padding: const EdgeInsets.all(16),
          children: const [LoadingSkeleton(height: 100, borderRadius: 20)],
        ),
        error: (_, _) => const Center(child: Text("Couldn't load this group.")),
        data: (group) {
          final members = membersAsync.valueOrNull ?? const [];
          final myRole = members.where((m) => m.userId == currentUserId).firstOrNull?.role;
          final isAdmin = myRole == GroupRole.admin;

          final events = eventsAsync.valueOrNull ?? const [];
          final joinRequests = isAdmin ? ref.watch(groupJoinRequestsProvider(groupId)).valueOrNull ?? const [] : const [];

          final friendIds = (ref.watch(friendsListProvider).valueOrNull ?? const [])
              .map((f) => f.profile.id)
              .toSet();
          final pendingFriendRequestIds = (ref.watch(outgoingFriendRequestsProvider).valueOrNull ?? const [])
              .map((r) => r.profile.id)
              .toSet();

          return RefreshIndicator(
            onRefresh: () async {
              ref.invalidate(groupMembersProvider(groupId));
              ref.invalidate(groupEventsProvider(groupId));
              if (isAdmin) ref.invalidate(groupJoinRequestsProvider(groupId));
            },
            child: ListView(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 32),
              children: [
                Text(group.name, style: Theme.of(context).textTheme.headlineSmall),
                if (group.description != null && group.description!.isNotEmpty) ...[
                  const SizedBox(height: 6),
                  Text(group.description!, style: Theme.of(context).textTheme.bodyLarge),
                ],
                const SizedBox(height: 20),
                Row(
                  children: [
                    Expanded(
                      child: AppButton(
                        label: 'Smart Time Finder',
                        icon: Icons.auto_awesome_rounded,
                        onPressed: () => context.push('/groups/$groupId/smart-time-finder'),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 10),
                Row(
                  children: [
                    Expanded(
                      child: AppButton(
                        label: 'Create event',
                        variant: AppButtonVariant.secondary,
                        icon: Icons.add_circle_outline_rounded,
                        onPressed: () => context.push('/groups/$groupId/events/create'),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 24),
                Text('Events', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 10),
                if (eventsAsync.isLoading)
                  const LoadingSkeleton(height: 76, borderRadius: 20)
                else if (events.isEmpty)
                  Text('No events yet.', style: Theme.of(context).textTheme.bodyMedium)
                else
                  for (final event in events) ...[
                    EventTile(event: event, onTap: () => context.push('/events/${event.id}')),
                    const SizedBox(height: 8),
                  ],
                if (isAdmin && joinRequests.isNotEmpty) ...[
                  const SizedBox(height: 24),
                  Text('Join requests (${joinRequests.length})', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 10),
                  for (final request in joinRequests) ...[
                    AppCard(
                      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                      child: Row(
                        children: [
                          ProfileAvatar(
                            displayName: request.profile.displayName,
                            imageUrl: request.profile.profilePictureUrl,
                            size: 40,
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(request.profile.displayName, style: Theme.of(context).textTheme.titleMedium),
                                Text('@${request.profile.username}', style: Theme.of(context).textTheme.bodyMedium),
                              ],
                            ),
                          ),
                          IconButton(
                            icon: const Icon(Icons.check_circle_rounded, color: AppColors.availableGreen),
                            onPressed: () => ref
                                .read(groupJoinRequestsProvider(groupId).notifier)
                                .respond(groupId, request.id, GroupJoinRequestResponse.accepted),
                          ),
                          IconButton(
                            icon: const Icon(Icons.cancel_rounded, color: AppColors.busyRed),
                            onPressed: () => ref
                                .read(groupJoinRequestsProvider(groupId).notifier)
                                .respond(groupId, request.id, GroupJoinRequestResponse.declined),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 8),
                  ],
                ],
                const SizedBox(height: 24),
                Text('Members (${members.length})', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 10),
                for (final member in members) ...[
                  AppCard(
                    padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                    child: Row(
                      children: [
                        ProfileAvatar(displayName: member.displayName, imageUrl: member.profilePictureUrl, size: 40),
                        const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(member.displayName, style: Theme.of(context).textTheme.titleMedium),
                              Text('@${member.username}', style: Theme.of(context).textTheme.bodyMedium),
                            ],
                          ),
                        ),
                        if (member.role == GroupRole.admin)
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                            decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(999)),
                            child: const Text('Admin', style: TextStyle(fontSize: 11.5, fontWeight: FontWeight.w700, color: AppColors.navy)),
                          ),
                        if (member.userId != currentUserId)
                          if (friendIds.contains(member.userId))
                            const Padding(
                              padding: EdgeInsets.only(left: 4),
                              child: Icon(Icons.check_circle_rounded, color: AppColors.availableGreen, size: 20),
                            )
                          else if (pendingFriendRequestIds.contains(member.userId))
                            const Padding(
                              padding: EdgeInsets.only(left: 4),
                              child: Icon(Icons.hourglass_top_rounded, color: AppColors.unknownGray, size: 20),
                            )
                          else
                            IconButton(
                              icon: const Icon(Icons.person_add_alt_1_rounded, color: AppColors.navy),
                              tooltip: 'Add friend',
                              onPressed: () async {
                                try {
                                  await ref.read(friendsRepositoryProvider).sendFriendRequest(addresseeId: member.userId);
                                  ref.invalidate(outgoingFriendRequestsProvider);
                                  if (context.mounted) {
                                    ScaffoldMessenger.of(context)
                                        .showSnackBar(const SnackBar(content: Text('Friend request sent.')));
                                  }
                                } catch (_) {
                                  if (context.mounted) {
                                    ScaffoldMessenger.of(context).showSnackBar(
                                      const SnackBar(content: Text("Couldn't send that request. Try again.")),
                                    );
                                  }
                                }
                              },
                            ),
                        if (isAdmin && member.userId != currentUserId)
                          PopupMenuButton<String>(
                            icon: const Icon(Icons.more_vert_rounded, color: AppColors.unknownGray),
                            onSelected: (action) => _handleMemberAction(context, ref, groupId, member, action),
                            itemBuilder: (context) => [
                              PopupMenuItem(
                                value: 'role',
                                child: Text(member.role == GroupRole.admin ? 'Remove admin' : 'Make admin'),
                              ),
                              const PopupMenuItem(value: 'remove', child: Text('Remove from group')),
                            ],
                          ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 8),
                ],
                const SizedBox(height: 16),
                AppButton(
                  label: 'Create invite link',
                  variant: AppButtonVariant.secondary,
                  icon: Icons.link_rounded,
                  onPressed: () async {
                    final invite = await ref.read(groupsRepositoryProvider).createInviteLink(groupId);
                    await Clipboard.setData(ClipboardData(text: invite.code));
                    if (context.mounted) {
                      final message = isAdmin
                          ? 'Invite code ${invite.code} copied to clipboard.'
                          : "Invite code ${invite.code} copied to clipboard. Since you're not an admin, people who join with it need admin approval.";
                      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(message)));
                    }
                  },
                ),
                const SizedBox(height: 10),
                AppButton(
                  label: 'Leave group',
                  variant: AppButtonVariant.text,
                  onPressed: () async {
                    await ref.read(myGroupsProvider.notifier).leaveGroup(groupId);
                    if (context.mounted) context.pop();
                  },
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Future<void> _handleMemberAction(
    BuildContext context,
    WidgetRef ref,
    String groupId,
    GroupMember member,
    String action,
  ) async {
    if (action == 'remove') {
      final confirmed = await showDialog<bool>(
        context: context,
        builder: (context) => AlertDialog(
          title: const Text('Remove member?'),
          content: Text('${member.displayName} will lose access to this group.'),
          actions: [
            TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
            TextButton(onPressed: () => Navigator.of(context).pop(true), child: const Text('Remove')),
          ],
        ),
      );
      if (confirmed != true) return;
    }

    try {
      if (action == 'remove') {
        await ref.read(groupsRepositoryProvider).removeMember(groupId, member.userId);
        // Removing changes the group's member count, which `myGroupsProvider` caches (shown on
        // the Groups tab's tiles) — that cache has no other reason to know this happened.
        await ref.read(myGroupsProvider.notifier).refresh();
      } else {
        final newRole = member.role == GroupRole.admin ? GroupRole.member : GroupRole.admin;
        await ref.read(groupsRepositoryProvider).changeMemberRole(groupId, member.userId, newRole);
      }
      ref.invalidate(groupMembersProvider(groupId));
    } catch (_) {
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("That didn't work — a group needs at least one admin.")),
        );
      }
    }
  }
}
