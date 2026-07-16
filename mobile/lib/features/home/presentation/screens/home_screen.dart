import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../../../core/widgets/availability_badge.dart';
import '../../../../core/widgets/empty_state.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../../availability/domain/entities/availability_block.dart';
import '../../../availability/presentation/providers/availability_providers.dart';
import '../../../friends/presentation/providers/friends_providers.dart';
import '../providers/home_providers.dart';
import '../widgets/friends_activity_strip.dart';
import '../widgets/suggested_meetup_card.dart';

String _greeting() {
  final hour = DateTime.now().hour;
  if (hour < 12) return 'Good morning';
  if (hour < 17) return 'Good afternoon';
  return 'Good evening';
}

class HomeScreen extends ConsumerWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final user = ref.watch(authControllerProvider).valueOrNull;
    final today = DateTime.now();
    final todayKey = DateRangeKey(DateTime(today.year, today.month, today.day), DateTime(today.year, today.month, today.day));
    final todayInstancesAsync = ref.watch(myInstancesProvider(todayKey));
    final upcomingAsync = ref.watch(upcomingScheduledEventsProvider);
    final suggestedMeetupAsync = ref.watch(suggestedMeetupProvider);
    final friendsAsync = ref.watch(friendsListProvider);

    return Scaffold(
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: () async {
            ref.invalidate(myInstancesProvider(todayKey));
            ref.invalidate(upcomingScheduledEventsProvider);
            ref.invalidate(suggestedMeetupProvider);
          },
          child: ListView(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
            children: [
              Row(
                children: [
                  ProfileAvatar(displayName: user?.displayName ?? '', size: 52),
                  const SizedBox(width: 14),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('${_greeting()},', style: Theme.of(context).textTheme.bodyMedium),
                        Text(user?.displayName ?? '', style: Theme.of(context).textTheme.headlineSmall),
                      ],
                    ),
                  ),
                  IconButton(
                    onPressed: () => context.push('/notifications'),
                    icon: const Icon(Icons.notifications_none_rounded),
                    style: IconButton.styleFrom(backgroundColor: AppColors.fog),
                  ),
                  const SizedBox(width: 8),
                  IconButton(
                    onPressed: () => ref.read(authControllerProvider.notifier).logout(),
                    icon: const Icon(Icons.logout_rounded),
                    style: IconButton.styleFrom(backgroundColor: AppColors.fog),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              Text('Today', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(height: 10),
              todayInstancesAsync.when(
                loading: () => const LoadingSkeleton(height: 72, borderRadius: 20),
                error: (_, _) => const SizedBox.shrink(),
                data: (instances) {
                  final declared = instances.where((i) => i.status != AvailabilityStatus.unknown).toList()
                    ..sort((a, b) => a.startTime.compareTo(b.startTime));
                  final blocks = mergeAvailabilityInstances(declared);

                  return AppCard(
                    padding: const EdgeInsets.all(16),
                    child: blocks.isEmpty
                        ? Text("You haven't set today's availability yet.", style: Theme.of(context).textTheme.bodyMedium)
                        : Wrap(
                            spacing: 8,
                            runSpacing: 8,
                            children: [
                              for (final block in blocks)
                                Chip(
                                  label: Text('${block.start.label} - ${block.end.label}'),
                                  avatar: CircleAvatar(
                                    backgroundColor: Colors.transparent,
                                    child: AvailabilityBadge(status: block.status, dense: true),
                                  ),
                                  backgroundColor: AppColors.fog,
                                  side: BorderSide.none,
                                ),
                            ],
                          ),
                  );
                },
              ),
              const SizedBox(height: 24),
              suggestedMeetupAsync.when(
                loading: () => const LoadingSkeleton(height: 110, borderRadius: 24),
                error: (_, _) => const SizedBox.shrink(),
                data: (suggestion) {
                  if (suggestion == null) return const SizedBox.shrink();
                  final (group, window) = suggestion;
                  return Padding(
                    padding: const EdgeInsets.only(bottom: 24),
                    child: SuggestedMeetupCard(
                      group: group,
                      window: window,
                      onTap: () => context.push('/groups/${group.id}/smart-time-finder'),
                    ),
                  );
                },
              ),
              Text('Upcoming events', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(height: 10),
              upcomingAsync.when(
                loading: () => const LoadingSkeleton(height: 72, borderRadius: 20),
                error: (_, _) => const SizedBox.shrink(),
                data: (events) {
                  if (events.isEmpty) {
                    return const EmptyState(
                      icon: Icons.event_outlined,
                      title: 'Nothing scheduled',
                      message: 'Confirmed events from your groups will show up here.',
                    );
                  }
                  return Column(
                    children: [
                      for (final scheduled in events.take(3)) ...[
                        AppCard(
                          onTap: () => context.push('/events/${scheduled.event.id}'),
                          padding: const EdgeInsets.all(14),
                          child: Row(
                            children: [
                              Container(
                                width: 44,
                                height: 44,
                                decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(12)),
                                child: const Icon(Icons.event_rounded, color: AppColors.accentBlue),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(scheduled.event.title, style: Theme.of(context).textTheme.titleMedium),
                                    Text(
                                      '${scheduled.groupName} · ${DateFormat('MMM d, h:mm a').format(scheduled.startUtc.toLocal())}',
                                      style: Theme.of(context).textTheme.bodyMedium,
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 10),
                      ],
                    ],
                  );
                },
              ),
              const SizedBox(height: 24),
              Text('Friends activity', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(height: 10),
              friendsAsync.when(
                loading: () => const LoadingSkeleton(height: 90, borderRadius: 20),
                error: (_, _) => const SizedBox.shrink(),
                data: (friends) => FriendsActivityStrip(friends: friends),
              ),
              const SizedBox(height: 24),
              Text('Quick actions', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(height: 10),
              Row(
                children: [
                  _QuickAction(icon: Icons.person_add_alt_1_rounded, label: 'Add friend', onTap: () => context.go('/friends')),
                  _QuickAction(icon: Icons.group_add_rounded, label: 'New group', onTap: () => context.go('/groups')),
                  _QuickAction(icon: Icons.event_available_rounded, label: 'Calendar', onTap: () => context.go('/calendar')),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _QuickAction extends StatelessWidget {
  const _QuickAction({required this.icon, required this.label, required this.onTap});

  final IconData icon;
  final String label;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(16),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12),
          child: Column(
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(color: AppColors.fog, shape: BoxShape.circle),
                child: Icon(icon, color: AppColors.navy),
              ),
              const SizedBox(height: 8),
              Text(label, textAlign: TextAlign.center, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600)),
            ],
          ),
        ),
      ),
    );
  }
}
