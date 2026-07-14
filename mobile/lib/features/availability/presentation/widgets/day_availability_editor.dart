import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/availability_badge.dart';
import '../providers/availability_providers.dart';

const _editableDayparts = [Daypart.morning, Daypart.afternoon, Daypart.evening, Daypart.night];

/// Bottom sheet for setting a single day's per-daypart availability — opened either from the
/// Calendar FAB (today) or by tapping a day in month view.
class DayAvailabilityEditor extends ConsumerWidget {
  const DayAvailabilityEditor({required this.date, required this.rangeKey, super.key});

  final DateTime date;
  final DateRangeKey rangeKey;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final instancesAsync = ref.watch(myInstancesProvider(rangeKey));
    final instances = instancesAsync.valueOrNull ?? const [];

    AvailabilityStatus statusFor(Daypart daypart) {
      final match = instances.where((i) =>
          i.date.year == date.year && i.date.month == date.month && i.date.day == date.day && i.daypart == daypart);
      return match.isEmpty ? AvailabilityStatus.unknown : match.first.status;
    }

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(999)),
              ),
            ),
            const SizedBox(height: 16),
            Text(DateFormat('EEEE, MMMM d').format(date), style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 4),
            Text('Tap a daypart to cycle its status.', style: Theme.of(context).textTheme.bodyMedium),
            const SizedBox(height: 20),
            for (final daypart in _editableDayparts) ...[
              _DaypartRow(
                daypart: daypart,
                status: statusFor(daypart),
                onTap: () {
                  final next = nextStatusFor(statusFor(daypart));
                  ref.read(myInstancesProvider(rangeKey).notifier).setStatus(date, daypart, next);
                },
              ),
              const SizedBox(height: 10),
            ],
          ],
        ),
      ),
    );
  }

  static AvailabilityStatus nextStatusFor(AvailabilityStatus current) {
    const order = [
      AvailabilityStatus.unknown,
      AvailabilityStatus.available,
      AvailabilityStatus.maybe,
      AvailabilityStatus.busy,
    ];
    return order[(order.indexOf(current) + 1) % order.length];
  }
}

class _DaypartRow extends StatelessWidget {
  const _DaypartRow({required this.daypart, required this.status, required this.onTap});

  final Daypart daypart;
  final AvailabilityStatus status;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(14),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(14)),
        child: Row(
          children: [
            Expanded(child: Text(daypart.label, style: Theme.of(context).textTheme.titleMedium)),
            AvailabilityBadge(status: status),
          ],
        ),
      ),
    );
  }
}
