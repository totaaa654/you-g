import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_card.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../friends/presentation/providers/friends_providers.dart';
import '../../../groups/presentation/providers/groups_providers.dart';
import '../providers/profile_providers.dart';
import '../providers/profile_stats_providers.dart';
import '../widgets/edit_profile_sheet.dart';

class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final profileAsync = ref.watch(myProfileProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Profile'),
        actions: [
          IconButton(onPressed: () => context.push('/settings'), icon: const Icon(Icons.settings_outlined)),
        ],
      ),
      body: profileAsync.when(
        loading: () => ListView(
          padding: const EdgeInsets.all(16),
          children: const [LoadingSkeleton(height: 160, borderRadius: 24)],
        ),
        error: (_, _) => const Center(child: Text("Couldn't load your profile.")),
        data: (profile) {
          final friendsCount = ref.watch(friendsListProvider).valueOrNull?.length;
          final groupsCount = ref.watch(myGroupsProvider).valueOrNull?.length;
          final eventsJoinedAsync = ref.watch(eventsJoinedCountProvider);
          final availabilityScoreAsync = ref.watch(availabilityScoreProvider);

          return RefreshIndicator(
            onRefresh: () async {
              ref.invalidate(myProfileProvider);
              ref.invalidate(eventsJoinedCountProvider);
              ref.invalidate(availabilityScoreProvider);
            },
            child: ListView(
              padding: const EdgeInsets.fromLTRB(16, 8, 16, 32),
              children: [
                Center(
                  child: Column(
                    children: [
                      ProfileAvatar(displayName: profile.displayName, imageUrl: profile.profilePictureUrl, size: 88),
                      const SizedBox(height: 12),
                      Text(profile.displayName, style: Theme.of(context).textTheme.headlineSmall),
                      Text('@${profile.username}', style: Theme.of(context).textTheme.bodyMedium),
                      const SizedBox(height: 4),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(999)),
                        child: Text(profile.friendCode, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w700)),
                      ),
                      if (profile.bio != null && profile.bio!.isNotEmpty) ...[
                        const SizedBox(height: 12),
                        Text(profile.bio!, textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodyLarge),
                      ],
                      const SizedBox(height: 16),
                      AppButton(
                        label: 'Edit profile',
                        variant: AppButtonVariant.secondary,
                        onPressed: () => showModalBottomSheet(
                          context: context,
                          isScrollControlled: true,
                          backgroundColor: Colors.white,
                          shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
                          builder: (context) => EditProfileSheet(profile: profile),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 28),
                Row(
                  children: [
                    _StatTile(label: 'Friends', value: friendsCount?.toString() ?? '—'),
                    _StatTile(label: 'Groups', value: groupsCount?.toString() ?? '—'),
                    _StatTile(label: 'Events joined', value: eventsJoinedAsync.valueOrNull?.toString() ?? '—'),
                  ],
                ),
                const SizedBox(height: 12),
                AppCard(
                  padding: const EdgeInsets.all(16),
                  child: Row(
                    children: [
                      const Icon(Icons.trending_up_rounded, color: AppColors.availableGreen),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text('Availability score', style: Theme.of(context).textTheme.titleMedium),
                            Text(
                              'Share of the next 7 days you\'ve marked Available',
                              style: Theme.of(context).textTheme.bodyMedium,
                            ),
                          ],
                        ),
                      ),
                      Text(
                        availabilityScoreAsync.valueOrNull == null
                            ? '—'
                            : '${(availabilityScoreAsync.valueOrNull! * 100).round()}%',
                        style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w800, color: AppColors.navy),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                Text('Achievements', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 10),
                AppCard(
                  padding: const EdgeInsets.all(20),
                  child: Column(
                    children: [
                      const Icon(Icons.emoji_events_outlined, color: AppColors.unknownGray, size: 32),
                      const SizedBox(height: 8),
                      Text(
                        'Achievements are coming soon',
                        style: Theme.of(context).textTheme.bodyMedium,
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}

class _StatTile extends StatelessWidget {
  const _StatTile({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: AppCard(
        padding: const EdgeInsets.symmetric(vertical: 16),
        child: Column(
          children: [
            Text(value, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.w800, color: AppColors.navy)),
            const SizedBox(height: 4),
            Text(label, style: const TextStyle(fontSize: 11.5, color: AppColors.unknownGray)),
          ],
        ),
      ),
    );
  }
}
