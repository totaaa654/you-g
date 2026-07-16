import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../groups/domain/entities/group_member.dart';
import '../../domain/entities/merged_overlap_window.dart';
import '../../domain/entities/overlap_window.dart';

/// Who's available/maybe/busy for one date, as a chronological list of merged time ranges
/// (contiguous 30-minute slots with the same set of people collapse into one range).
class GroupDayBreakdownSheet extends StatelessWidget {
  const GroupDayBreakdownSheet({required this.date, required this.windowsForDay, required this.members, super.key});

  final DateTime date;
  final List<OverlapWindow> windowsForDay;
  final List<GroupMember> members;

  @override
  Widget build(BuildContext context) {
    final merged = mergeOverlapWindows(windowsForDay);

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
            child: merged.isEmpty
                ? Center(
                    child: Text('No one has marked themselves available.', style: Theme.of(context).textTheme.bodyMedium),
                  )
                : ListView(
                    controller: scrollController,
                    padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
                    children: [
                      for (final window in merged) ...[
                        _WindowSection(window: window, members: members),
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

class _WindowSection extends StatelessWidget {
  const _WindowSection({required this.window, required this.members});

  final MergedOverlapWindow window;
  final List<GroupMember> members;

  @override
  Widget build(BuildContext context) {
    final availableMembers = members.where((m) => window.availableUserIds.contains(m.userId)).toList();
    final maybeMembers = members.where((m) => window.maybeUserIds.contains(m.userId)).toList();
    final busyMembers = members.where((m) => !availableMembers.contains(m) && !maybeMembers.contains(m)).toList();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('${window.startTime.label} - ${window.endTime.label}', style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 8),
        if (availableMembers.isNotEmpty) _MemberRow(members: availableMembers, color: AppColors.availableGreen),
        if (maybeMembers.isNotEmpty) ...[
          const SizedBox(height: 6),
          _MemberRow(members: maybeMembers, color: AppColors.maybeGold),
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
