import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/availability_badge.dart';
import '../providers/availability_providers.dart';
import '../widgets/day_availability_editor.dart';

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

  @override
  Widget build(BuildContext context) {
    final monthRange = DateRangeKey(
      DateTime(_focusedMonth.year, _focusedMonth.month, 1),
      DateTime(_focusedMonth.year, _focusedMonth.month + 1, 0),
    );
    final weekRange = DateRangeKey(_focusedWeekStart, _focusedWeekStart.add(const Duration(days: 6)));
    final activeRange = _mode == _CalendarViewMode.month ? monthRange : weekRange;

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
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _openEditor(DateTime.now(), activeRange),
        icon: const Icon(Icons.edit_calendar_rounded),
        label: const Text('Update availability'),
      ),
      body: _mode == _CalendarViewMode.month
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
            ),
    );
  }
}

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

const _weekDayparts = [Daypart.morning, Daypart.afternoon, Daypart.evening, Daypart.night];

class _WeekView extends ConsumerWidget {
  const _WeekView({required this.weekStart, required this.rangeKey, required this.onPrev, required this.onNext});

  final DateTime weekStart;
  final DateRangeKey rangeKey;
  final VoidCallback onPrev;
  final VoidCallback onNext;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final instancesAsync = ref.watch(myInstancesProvider(rangeKey));
    final instances = instancesAsync.valueOrNull ?? const [];
    final days = List.generate(7, (i) => weekStart.add(Duration(days: i)));

    AvailabilityStatus statusFor(DateTime day, Daypart daypart) {
      final match = instances.where(
        (i) => i.date.year == day.year && i.date.month == day.month && i.date.day == day.day && i.daypart == daypart,
      );
      return match.isEmpty ? AvailabilityStatus.unknown : match.first.status;
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
          child: SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
            child: Table(
              defaultColumnWidth: const FixedColumnWidth(56),
              children: [
                TableRow(
                  children: [
                    const SizedBox(),
                    for (final day in days)
                      Padding(
                        padding: const EdgeInsets.symmetric(vertical: 6),
                        child: Column(
                          children: [
                            Text(DateFormat('E').format(day), style: const TextStyle(fontSize: 11, color: AppColors.unknownGray)),
                            Text('${day.day}', style: const TextStyle(fontWeight: FontWeight.w700)),
                          ],
                        ),
                      ),
                  ],
                ),
                for (final daypart in _weekDayparts)
                  TableRow(
                    children: [
                      Padding(
                        padding: const EdgeInsets.symmetric(vertical: 10),
                        child: Text(daypart.label, style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w600)),
                      ),
                      for (final day in days)
                        Padding(
                          padding: const EdgeInsets.all(4),
                          child: InkWell(
                            borderRadius: BorderRadius.circular(10),
                            onTap: () {
                              final next = DayAvailabilityEditor.nextStatusFor(statusFor(day, daypart));
                              ref.read(myInstancesProvider(rangeKey).notifier).setStatus(day, daypart, next);
                            },
                            child: Center(child: AvailabilityBadge(status: statusFor(day, daypart), dense: true)),
                          ),
                        ),
                    ],
                  ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}
