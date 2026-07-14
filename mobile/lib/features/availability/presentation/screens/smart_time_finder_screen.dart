import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/empty_state.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../../../core/widgets/mascot.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../groups/domain/entities/group_member.dart';
import '../../../groups/presentation/providers/groups_providers.dart';
import '../providers/availability_providers.dart';
import '../widgets/overlap_window_card.dart';

class SmartTimeFinderScreen extends ConsumerStatefulWidget {
  const SmartTimeFinderScreen({required this.groupId, super.key});

  final String groupId;

  @override
  ConsumerState<SmartTimeFinderScreen> createState() => _SmartTimeFinderScreenState();
}

class _SmartTimeFinderScreenState extends ConsumerState<SmartTimeFinderScreen> {
  bool _weekendOnly = false;
  int _horizonDays = 14;

  @override
  Widget build(BuildContext context) {
    final now = DateTime.now();
    final query = GroupOverlapQuery(
      groupId: widget.groupId,
      from: DateTime(now.year, now.month, now.day),
      to: DateTime(now.year, now.month, now.day).add(Duration(days: _horizonDays)),
      weekendOnly: _weekendOnly,
    );
    final overlapAsync = ref.watch(groupOverlapProvider(query));
    final membersAsync = ref.watch(groupMembersProvider(widget.groupId));

    return Scaffold(
      appBar: AppBar(title: const Text('Smart Time Finder')),
      body: overlapAsync.when(
        loading: () => ListView(
          padding: const EdgeInsets.all(16),
          children: const [
            LoadingSkeleton(height: 140, borderRadius: 20),
            SizedBox(height: 12),
            LoadingSkeleton(height: 90, borderRadius: 20),
          ],
        ),
        error: (_, _) =>
            const EmptyState(icon: Icons.error_outline_rounded, title: "Couldn't compute overlap", message: 'Try again in a moment.'),
        data: (windows) {
          if (windows.isEmpty) {
            return const EmptyState(
              icon: Icons.auto_awesome_outlined,
              title: 'No overlap yet',
              message: 'Once group members set their availability, the best times to meet will show up here.',
            );
          }

          final sorted = [...windows]..sort((a, b) => b.availableCount.compareTo(a.availableCount));
          final best = sorted.first;
          final alternatives = sorted.skip(1).take(4).toList();
          final members = membersAsync.valueOrNull ?? const [];

          final availableMembers = members.where((m) => best.availableUserIds.contains(m.userId)).toList();
          final maybeMembers = members.where((m) => best.maybeUserIds.contains(m.userId)).toList();
          final busyMembers = members
              .where((m) => !best.availableUserIds.contains(m.userId) && !best.maybeUserIds.contains(m.userId))
              .toList();

          return ListView(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 32),
            children: [
              const Center(child: Mascot(size: 100)),
              const SizedBox(height: 12),
              Center(
                child: Text(
                  "Here's the best time to meet up",
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.titleLarge,
                ),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  FilterChip(
                    label: const Text('Weekends only'),
                    selected: _weekendOnly,
                    onSelected: (v) => setState(() => _weekendOnly = v),
                  ),
                  const SizedBox(width: 8),
                  for (final horizon in [7, 14, 30])
                    Padding(
                      padding: const EdgeInsets.only(right: 8),
                      child: ChoiceChip(
                        label: Text('${horizon}d'),
                        selected: _horizonDays == horizon,
                        onSelected: (_) => setState(() => _horizonDays = horizon),
                      ),
                    ),
                ],
              ),
              const SizedBox(height: 20),
              Text('Best suggested time', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(height: 10),
              OverlapWindowCard(window: best, highlighted: true),
              const SizedBox(height: 20),
              _MemberSection(title: 'Available', members: availableMembers, color: AppColors.availableGreen),
              if (maybeMembers.isNotEmpty) ...[
                const SizedBox(height: 12),
                _MemberSection(title: 'Maybe', members: maybeMembers, color: AppColors.maybeGold),
              ],
              if (busyMembers.isNotEmpty) ...[
                const SizedBox(height: 12),
                _MemberSection(title: 'Busy', members: busyMembers, color: AppColors.busyRed),
              ],
              const SizedBox(height: 24),
              AppButton(
                label: 'Create event at this time',
                icon: Icons.add_circle_outline_rounded,
                onPressed: () => context.push(
                  '/groups/${widget.groupId}/events/create'
                  '?date=${best.date.toIso8601String()}&daypart=${best.daypart.toJson()}',
                ),
              ),
              if (alternatives.isNotEmpty) ...[
                const SizedBox(height: 28),
                Text('Alternative suggestions', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 10),
                for (final window in alternatives) ...[
                  OverlapWindowCard(
                    window: window,
                    onTap: () => context.push(
                      '/groups/${widget.groupId}/events/create'
                      '?date=${window.date.toIso8601String()}&daypart=${window.daypart.toJson()}',
                    ),
                  ),
                  const SizedBox(height: 10),
                ],
              ],
            ],
          );
        },
      ),
    );
  }
}

class _MemberSection extends StatelessWidget {
  const _MemberSection({required this.title, required this.members, required this.color});

  final String title;
  final List<GroupMember> members;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Container(width: 8, height: 8, decoration: BoxDecoration(color: color, shape: BoxShape.circle)),
            const SizedBox(width: 8),
            Text('$title (${members.length})', style: Theme.of(context).textTheme.titleMedium),
          ],
        ),
        const SizedBox(height: 8),
        SizedBox(
          height: 64,
          child: ListView.separated(
            scrollDirection: Axis.horizontal,
            itemCount: members.length,
            separatorBuilder: (context, index) => const SizedBox(width: 10),
            itemBuilder: (context, index) {
              final member = members[index];
              return Column(
                children: [
                  ProfileAvatar(displayName: member.displayName, imageUrl: member.profilePictureUrl, size: 40),
                ],
              );
            },
          ),
        ),
      ],
    );
  }
}
