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
import '../../domain/entities/group_role.dart';
import '../providers/groups_providers.dart';

/// Not part of bottom nav — the hub Smart Time Finder / Create Event / member management
/// are reached from. The "recent activity" events section gets added once the Events
/// feature exists (this screen is revisited then, per the build plan).
class GroupDetailScreen extends ConsumerWidget {
  const GroupDetailScreen({required this.groupId, super.key});

  final String groupId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final groupAsync = ref.watch(groupByIdProvider(groupId));
    final membersAsync = ref.watch(groupMembersProvider(groupId));
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

          return RefreshIndicator(
            onRefresh: () async => ref.invalidate(groupMembersProvider(groupId)),
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
                      ],
                    ),
                  ),
                  const SizedBox(height: 8),
                ],
                const SizedBox(height: 16),
                if (isAdmin)
                  AppButton(
                    label: 'Create invite link',
                    variant: AppButtonVariant.secondary,
                    icon: Icons.link_rounded,
                    onPressed: () async {
                      final invite = await ref.read(groupsRepositoryProvider).createInviteLink(groupId);
                      await Clipboard.setData(ClipboardData(text: invite.code));
                      if (context.mounted) {
                        ScaffoldMessenger.of(context)
                            .showSnackBar(SnackBar(content: Text('Invite code ${invite.code} copied to clipboard.')));
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
}
