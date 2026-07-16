import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../groups/domain/entities/group_member.dart';
import '../../domain/entities/overlap_window.dart';

/// Who's available/maybe/busy for one date, broken down by daypart — reuses the same
/// available/maybe/busy split Smart Time Finder computes, just scoped to a single day and
/// shown for every daypart at once instead of only the single best-ranked window.
class GroupDayBreakdownSheet extends StatelessWidget {
  const GroupDayBreakdownSheet({required this.date, required this.windowsForDay, required this.members, super.key});

  final DateTime date;
  final List<OverlapWindow> windowsForDay;
  final List<GroupMember> members;

  static const _dayparts = [Daypart.morning, Daypart.afternoon, Daypart.evening, Daypart.night];

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      initialChildSize: 0.7,
      minChildSize: 0.4,
      maxChildSize: 0.95,
      expand: false,
      builder: (context, scrollController) => Column(
        children: [
          const SizedBox(height: 12),
          Container(width: 40, height: 4, decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(999))),
          const SizedBox(height: 16),
          Text(DateFormat('EEEE, MMM d').format(date), style: Theme.of(context).textTheme.titleLarge),
          const SizedBox(height: 8),
          Expanded(
            child: ListView(
              controller: scrollController,
              padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
              children: [
                for (final daypart in _dayparts) ...[
                  _DaypartSection(
                    daypart: daypart,
                    window: windowsForDay.where((w) => w.daypart == daypart).firstOrNull,
                    members: members,
                  ),
                  const SizedBox(height: 16),
                ],
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _DaypartSection extends StatelessWidget {
  const _DaypartSection({required this.daypart, required this.window, required this.members});

  final Daypart daypart;
  final OverlapWindow? window;
  final List<GroupMember> members;

  @override
  Widget build(BuildContext context) {
    final availableMembers =
        window == null ? const <GroupMember>[] : members.where((m) => window!.availableUserIds.contains(m.userId)).toList();
    final maybeMembers =
        window == null ? const <GroupMember>[] : members.where((m) => window!.maybeUserIds.contains(m.userId)).toList();
    final busyMembers = members
        .where((m) => !availableMembers.contains(m) && !maybeMembers.contains(m))
        .toList();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(daypart.label, style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        if (availableMembers.isEmpty && maybeMembers.isEmpty)
          Text('No one has marked themselves available.', style: Theme.of(context).textTheme.bodyMedium)
        else ...[
          if (availableMembers.isNotEmpty) _MemberRow(members: availableMembers, color: AppColors.availableGreen),
          if (maybeMembers.isNotEmpty) ...[
            const SizedBox(height: 6),
            _MemberRow(members: maybeMembers, color: AppColors.maybeGold),
          ],
        ],
        if (busyMembers.isNotEmpty) ...[
          const SizedBox(height: 6),
          _MemberRow(members: busyMembers, color: AppColors.unknownGray),
        ],
      ],
    );
  }
}

class _MemberRow extends StatelessWidget {
  const _MemberRow({required this.members, required this.color});

  final List<GroupMember> members;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: [
        for (final member in members)
          Chip(
            avatar: ProfileAvatar(displayName: member.displayName, imageUrl: member.profilePictureUrl, size: 22),
            label: Text(member.displayName),
            backgroundColor: color.withValues(alpha: 0.12),
            side: BorderSide.none,
          ),
      ],
    );
  }
}
