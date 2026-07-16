import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/availability_badge.dart';
import '../../../groups/domain/entities/group_member.dart';
import '../../../groups/presentation/providers/groups_providers.dart';
import '../../domain/entities/availability_block.dart';
import '../../domain/entities/overlap_window.dart';
import '../providers/availability_providers.dart';
import '../widgets/day_availability_editor.dart';
import '../widgets/group_day_breakdown_sheet.dart';

enum _CalendarViewMode { month, week }

class CalendarScreen extends ConsumerStatefulWidget {
  const CalendarScreen({super.key});

  @override
  ConsumerState<CalendarScreen> createState() => _CalendarScreenState();
}

class _CalendarScreenState extends ConsumerState<CalendarScreen> {
  _CalendarViewMode _mode = _CalendarViewMode.month;
  DateTime _focusedMonth = DateTime(DateTime.now().year, DateTime.now().month);
  DateTime _focusedWeekStart = _startOfWeek(DateTime.now());
  String? _selectedGroupId;

  static DateTime _startOfWeek(DateTime date) => DateTime(date.year, date.month, date.day - (date.weekday - 1));

  void _openEditor(DateTime date, DateRangeKey rangeKey) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
      builder: (context) => DayAvailabilityEditor(date: date, rangeKey: rangeKey),
    );
  }

  void _openGroupDayBreakdown(DateTime date, List<OverlapWindow> windows, List<GroupMember> members) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
      builder: (context) => GroupDayBreakdownSheet(
        date: date,
        windowsForDay: windows.where((w) => _isSameDay(w.date, date)).toList(),
        members: members,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final monthRange = DateRangeKey(
      DateTime(_focusedMonth.year, _focusedMonth.month, 1),
      DateTime(_focusedMonth.year, _focusedMonth.month + 1, 0),
    );
    final weekRange = DateRangeKey(_focusedWeekStart, _focusedWeekStart.add(const Duration(days: 6)));
    final activeRange = _mode == _CalendarViewMode.month ? monthRange : weekRange;
    final groupId = _selectedGroupId;
    final groups = ref.watch(myGroupsProvider).valueOrNull ?? const [];

    return Scaffold(
      appBar: AppBar(
        title: const Text('Calendar'),
        actions: [
          Padding(
            padding: const EdgeInsets.only(right: 12),
            child: SegmentedButton<_CalendarViewMode>(
              segments: const [
                ButtonSegment(value: _CalendarViewMode.month, label: Text('Month')),
                ButtonSegment(value: _CalendarViewMode.week, label: Text('Week')),
              ],
              selected: {_mode},
              onSelectionChanged: (selection) => setState(() => _mode = selection.first),
            ),
          ),
        ],
      ),
      floatingActionButton: groupId == null
          ? FloatingActionButton.extended(
              onPressed: () => _openEditor(DateTime.now(), activeRange),
              icon: const Icon(Icons.edit_calendar_rounded),
              label: const Text('Update availability'),
            )
          : null,
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 10, 16, 0),
            child: DropdownButtonFormField<String?>(
              initialValue: groupId,
              decoration: const InputDecoration(
                labelText: 'Viewing',
                prefixIcon: Icon(Icons.groups_rounded),
                isDense: true,
              ),
              items: [
                const DropdownMenuItem(value: null, child: Text('My availability')),
                for (final group in groups) DropdownMenuItem(value: group.id, child: Text(group.name)),
              ],
              onChanged: (value) => setState(() => _selectedGroupId = value),
            ),
          ),
          Expanded(
            child: groupId == null
                ? (_mode == _CalendarViewMode.month
                    ? _MonthView(
                        month: _focusedMonth,
                        rangeKey: monthRange,
                        onPrev: () => setState(() => _focusedMonth = DateTime(_focusedMonth.year, _focusedMonth.month - 1)),
                        onNext: () => setState(() => _focusedMonth = DateTime(_focusedMonth.year, _focusedMonth.month + 1)),
                        onDayTap: (date) => _openEditor(date, monthRange),
                      )
                    : _WeekView(
                        weekStart: _focusedWeekStart,
                        rangeKey: weekRange,
                        onPrev: () => setState(() => _focusedWeekStart = _focusedWeekStart.subtract(const Duration(days: 7))),
                        onNext: () => setState(() => _focusedWeekStart = _focusedWeekStart.add(const Duration(days: 7))),
                        onDayTap: (date) => _openEditor(date, weekRange),
                      ))
                : (_mode == _CalendarViewMode.month
                    ? _GroupMonthView(
                        groupId: groupId,
                        month: _focusedMonth,
                        onPrev: () => setState(() => _focusedMonth = DateTime(_focusedMonth.year, _focusedMonth.month - 1)),
                        onNext: () => setState(() => _focusedMonth = DateTime(_focusedMonth.year, _focusedMonth.month + 1)),
                        onDayTap: _openGroupDayBreakdown,
                      )
                    : _GroupWeekView(
                        groupId: groupId,
                        weekStart: _focusedWeekStart,
                        onPrev: () => setState(() => _focusedWeekStart = _focusedWeekStart.subtract(const Duration(days: 7))),
                        onNext: () => setState(() => _focusedWeekStart = _focusedWeekStart.add(const Duration(days: 7))),
                        onDayTap: _openGroupDayBreakdown,
                      )),
          ),
        ],
      ),
    );
  }
}

bool _isSameDay(DateTime a, DateTime b) => a.year == b.year && a.month == b.month && a.day == b.day;

class _MonthView extends ConsumerWidget {
  const _MonthView({
    required this.month,
    required this.rangeKey,
    required this.onPrev,
    required this.onNext,
    required this.onDayTap,
  });

  final DateTime month;
  final DateRangeKey rangeKey;
  final VoidCallback onPrev;
  final VoidCallback onNext;
  final ValueChanged<DateTime> onDayTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final instancesAsync = ref.watch(myInstancesProvider(rangeKey));
    final instances = instancesAsync.valueOrNull ?? const [];

    final firstOfMonth = DateTime(month.year, month.month, 1);
    final leadingBlanks = firstOfMonth.weekday - 1;
    final daysInMonth = DateTime(month.year, month.month + 1, 0).day;
    final today = DateTime.now();

    AvailabilityStatus? dominantStatus(DateTime day) {
      final dayInstances = instances.where(
        (i) => i.date.year == day.year && i.date.month == day.month && i.date.day == day.day,
      );
      if (dayInstances.isEmpty) return null;
      if (dayInstances.any((i) => i.status == AvailabilityStatus.available)) return AvailabilityStatus.available;
      if (dayInstances.any((i) => i.status == AvailabilityStatus.maybe)) return AvailabilityStatus.maybe;
      if (dayInstances.any((i) => i.status == AvailabilityStatus.busy)) return AvailabilityStatus.busy;
      return AvailabilityStatus.unknown;
    }

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              IconButton(onPressed: onPrev, icon: const Icon(Icons.chevron_left_rounded)),
              Text(DateFormat('MMMM yyyy').format(month), style: Theme.of(context).textTheme.titleMedium),
              IconButton(onPressed: onNext, icon: const Icon(Icons.chevron_right_rounded)),
            ],
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12),
          child: Row(
            children: [for (final d in const ['M', 'T', 'W', 'T', 'F', 'S', 'S']) Expanded(child: Center(child: Text(d, style: const TextStyle(color: AppColors.unknownGray, fontWeight: FontWeight.w700, fontSize: 12))))],
          ),
        ),
        const SizedBox(height: 4),
        Expanded(
          child: GridView.builder(
            padding: const EdgeInsets.all(12),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: 7),
            itemCount: leadingBlanks + daysInMonth,
            itemBuilder: (context, index) {
              if (index < leadingBlanks) return const SizedBox.shrink();
              final day = DateTime(month.year, month.month, index - leadingBlanks + 1);
              final isToday = day.year == today.year && day.month == today.month && day.day == today.day;
              final status = dominantStatus(day);

              return InkWell(
                onTap: () => onDayTap(day),
                borderRadius: BorderRadius.circular(12),
                child: Container(
                  margin: const EdgeInsets.all(3),
                  decoration: BoxDecoration(
                    color: isToday ? AppColors.navy.withValues(alpha: 0.08) : null,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text('${day.day}', style: TextStyle(fontWeight: isToday ? FontWeight.w800 : FontWeight.w500)),
                      const SizedBox(height: 4),
                      if (status != null) AvailabilityBadge(status: status, dense: true),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

class _WeekView extends ConsumerWidget {
  const _WeekView({required this.weekStart, required this.rangeKey, required this.onPrev, required this.onNext, required this.onDayTap});

  final DateTime weekStart;
  final DateRangeKey rangeKey;
  final VoidCallback onPrev;
  final VoidCallback onNext;
  final ValueChanged<DateTime> onDayTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final instancesAsync = ref.watch(myInstancesProvider(rangeKey));
    final instances = instancesAsync.valueOrNull ?? const [];
    final days = List.generate(7, (i) => weekStart.add(Duration(days: i)));

    List<AvailabilityBlock> blocksFor(DateTime day) {
      final dayInstances = instances.where((i) => _isSameDay(i.date, day) && i.status != AvailabilityStatus.unknown).toList()
        ..sort((a, b) => a.startTime.compareTo(b.startTime));
      return mergeAvailabilityInstances(dayInstances);
    }

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              IconButton(onPressed: onPrev, icon: const Icon(Icons.chevron_left_rounded)),
              Text(
                '${DateFormat('MMM d').format(days.first)} - ${DateFormat('MMM d').format(days.last)}',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              IconButton(onPressed: onNext, icon: const Icon(Icons.chevron_right_rounded)),
            ],
          ),
        ),
        Expanded(
          child: ListView(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
            children: [
              for (final day in days) ...[
                _WeekDayCard(day: day, blocks: blocksFor(day), onTap: () => onDayTap(day)),
                const SizedBox(height: 10),
              ],
            ],
          ),
        ),
      ],
    );
  }
}

class _WeekDayCard extends StatelessWidget {
  const _WeekDayCard({required this.day, required this.blocks, required this.onTap});

  final DateTime day;
  final List<AvailabilityBlock> blocks;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final isToday = _isSameDay(day, DateTime.now());
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: isToday ? AppColors.navy.withValues(alpha: 0.06) : AppColors.fog,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            SizedBox(
              width: 48,
              child: Column(
                children: [
                  Text(DateFormat('E').format(day), style: const TextStyle(fontSize: 11, color: AppColors.unknownGray)),
                  Text('${day.day}', style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 16)),
                ],
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: blocks.isEmpty
                  ? const Text('No availability set', style: TextStyle(color: AppColors.unknownGray, fontSize: 13))
                  : Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        for (final block in blocks)
                          Padding(
                            padding: const EdgeInsets.only(bottom: 4),
                            child: Row(
                              children: [
                                AvailabilityBadge(status: block.status, dense: true),
                                const SizedBox(width: 8),
                                Text('${block.start.label} - ${block.end.label}', style: const TextStyle(fontSize: 13)),
                              ],
                            ),
                          ),
                      ],
                    ),
            ),
            const Icon(Icons.chevron_right_rounded, color: AppColors.unknownGray),
          ],
        ),
      ),
    );
  }
}

class _GroupMonthView extends ConsumerWidget {
  const _GroupMonthView({
    required this.groupId,
    required this.month,
    required this.onPrev,
    required this.onNext,
    required this.onDayTap,
  });

  final String groupId;
  final DateTime month;
  final VoidCallback onPrev;
  final VoidCallback onNext;
  final void Function(DateTime date, List<OverlapWindow> windows, List<GroupMember> members) onDayTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final from = DateTime(month.year, month.month, 1);
    final to = DateTime(month.year, month.month + 1, 0);
    final windowsAsync = ref.watch(groupOverlapProvider(GroupOverlapQuery(groupId: groupId, from: from, to: to)));
    final windows = windowsAsync.valueOrNull ?? const [];
    final members = ref.watch(groupMembersProvider(groupId)).valueOrNull ?? const [];
    final totalMembers = members.length;

    final leadingBlanks = from.weekday - 1;
    final daysInMonth = to.day;
    final today = DateTime.now();

    double bestRatio(DateTime day) {
      final dayWindows = windows.where((w) => _isSameDay(w.date, day));
      if (dayWindows.isEmpty || totalMembers == 0) return 0;
      return dayWindows.map((w) => w.availableCount / totalMembers).reduce((a, b) => a > b ? a : b);
    }

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              IconButton(onPressed: onPrev, icon: const Icon(Icons.chevron_left_rounded)),
              Text(DateFormat('MMMM yyyy').format(month), style: Theme.of(context).textTheme.titleMedium),
              IconButton(onPressed: onNext, icon: const Icon(Icons.chevron_right_rounded)),
            ],
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12),
          child: Row(
            children: [for (final d in const ['M', 'T', 'W', 'T', 'F', 'S', 'S']) Expanded(child: Center(child: Text(d, style: const TextStyle(color: AppColors.unknownGray, fontWeight: FontWeight.w700, fontSize: 12))))],
          ),
        ),
        const SizedBox(height: 4),
        Expanded(
          child: GridView.builder(
            padding: const EdgeInsets.all(12),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: 7),
            itemCount: leadingBlanks + daysInMonth,
            itemBuilder: (context, index) {
              if (index < leadingBlanks) return const SizedBox.shrink();
              final day = DateTime(month.year, month.month, index - leadingBlanks + 1);
              final isToday = day.year == today.year && day.month == today.month && day.day == today.day;
              final ratio = bestRatio(day);
              final dotColor = ratio >= 0.5
                  ? AppColors.availableGreen
                  : ratio > 0
                      ? AppColors.maybeGold
                      : null;

              return InkWell(
                onTap: () => onDayTap(day, windows, members),
                borderRadius: BorderRadius.circular(12),
                child: Container(
                  margin: const EdgeInsets.all(3),
                  decoration: BoxDecoration(
                    color: isToday ? AppColors.navy.withValues(alpha: 0.08) : null,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text('${day.day}', style: TextStyle(fontWeight: isToday ? FontWeight.w800 : FontWeight.w500)),
                      const SizedBox(height: 4),
                      if (dotColor != null) Container(width: 8, height: 8, decoration: BoxDecoration(color: dotColor, shape: BoxShape.circle)),
                    ],
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}

class _GroupWeekView extends ConsumerWidget {
  const _GroupWeekView({
    required this.groupId,
    required this.weekStart,
    required this.onPrev,
    required this.onNext,
    required this.onDayTap,
  });

  final String groupId;
  final DateTime weekStart;
  final VoidCallback onPrev;
  final VoidCallback onNext;
  final void Function(DateTime date, List<OverlapWindow> windows, List<GroupMember> members) onDayTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final days = List.generate(7, (i) => weekStart.add(Duration(days: i)));
    final windowsAsync = ref.watch(
      groupOverlapProvider(GroupOverlapQuery(groupId: groupId, from: days.first, to: days.last)),
    );
    final windows = windowsAsync.valueOrNull ?? const [];
    final members = ref.watch(groupMembersProvider(groupId)).valueOrNull ?? const [];
    final totalMembers = members.length;

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              IconButton(onPressed: onPrev, icon: const Icon(Icons.chevron_left_rounded)),
              Text(
                '${DateFormat('MMM d').format(days.first)} - ${DateFormat('MMM d').format(days.last)}',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              IconButton(onPressed: onNext, icon: const Icon(Icons.chevron_right_rounded)),
            ],
          ),
        ),
        Expanded(
          child: ListView(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
            children: [
              for (final day in days) ...[
                _GroupWeekDayCard(
                  day: day,
                  windowsForDay: windows.where((w) => _isSameDay(w.date, day)).toList(),
                  totalMembers: totalMembers,
                  onTap: () => onDayTap(day, windows, members),
                ),
                const SizedBox(height: 10),
              ],
            ],
          ),
        ),
      ],
    );
  }
}

class _GroupWeekDayCard extends StatelessWidget {
  const _GroupWeekDayCard({required this.day, required this.windowsForDay, required this.totalMembers, required this.onTap});

  final DateTime day;
  final List<OverlapWindow> windowsForDay;
  final int totalMembers;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final isToday = _isSameDay(day, DateTime.now());
    final best = windowsForDay.isEmpty
        ? null
        : (windowsForDay.toList()..sort((a, b) => b.availableCount.compareTo(a.availableCount))).first;

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: isToday ? AppColors.navy.withValues(alpha: 0.06) : AppColors.fog,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Row(
          children: [
            SizedBox(
              width: 48,
              child: Column(
                children: [
                  Text(DateFormat('E').format(day), style: const TextStyle(fontSize: 11, color: AppColors.unknownGray)),
                  Text('${day.day}', style: const TextStyle(fontWeight: FontWeight.w700, fontSize: 16)),
                ],
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: best == null
                  ? const Text('No one available yet', style: TextStyle(color: AppColors.unknownGray, fontSize: 13))
                  : Text(
                      'Best: ${best.startTime.label} - ${best.startTime.next.label} · ${best.availableCount}/$totalMembers free',
                      style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
                    ),
            ),
            const Icon(Icons.chevron_right_rounded, color: AppColors.unknownGray),
          ],
        ),
      ),
    );
  }
}
