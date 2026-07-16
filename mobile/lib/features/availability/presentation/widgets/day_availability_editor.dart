import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/models/time_slot.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/availability_badge.dart';
import '../../domain/entities/availability_block.dart';
import '../providers/availability_providers.dart';

/// Bottom sheet for a single day's availability — lists whatever time blocks are already set,
/// and lets you add a new one by picking a status plus a start/end time (snapped to the nearest
/// 30 minutes, matching what the backend stores).
class DayAvailabilityEditor extends ConsumerStatefulWidget {
  const DayAvailabilityEditor({required this.date, required this.rangeKey, super.key});

  final DateTime date;
  final DateRangeKey rangeKey;

  @override
  ConsumerState<DayAvailabilityEditor> createState() => _DayAvailabilityEditorState();
}

class _DayAvailabilityEditorState extends ConsumerState<DayAvailabilityEditor> {
  AvailabilityStatus _status = AvailabilityStatus.available;
  TimeSlot _start = const TimeSlot(9 * 60);
  TimeSlot _end = const TimeSlot(17 * 60);
  bool _saving = false;

  Future<void> _pickTime({required bool isStart}) async {
    final initial = isStart ? _start : _end;
    final picked = await showTimePicker(
      context: context,
      initialTime: TimeOfDay(hour: initial.hour, minute: initial.minute),
    );
    if (picked == null) return;
    setState(() {
      final snapped = TimeSlot.fromMinutes(picked.hour, picked.minute);
      if (isStart) {
        _start = snapped;
        if (_end.minutesSinceMidnight <= _start.minutesSinceMidnight) _end = _start.next;
      } else {
        _end = snapped.minutesSinceMidnight > _start.minutesSinceMidnight ? snapped : _start.next;
      }
    });
  }

  Future<void> _addBlock() async {
    setState(() => _saving = true);
    try {
      await ref.read(myInstancesProvider(widget.rangeKey).notifier).setStatusRange(widget.date, _start, _end, _status);
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _clearBlock(TimeSlot start, TimeSlot end) =>
      ref.read(myInstancesProvider(widget.rangeKey).notifier).setStatusRange(widget.date, start, end, AvailabilityStatus.unknown);

  @override
  Widget build(BuildContext context) {
    final instancesAsync = ref.watch(myInstancesProvider(widget.rangeKey));
    final instances = instancesAsync.valueOrNull ?? const [];
    final dayInstances = instances
        .where((i) =>
            i.date.year == widget.date.year &&
            i.date.month == widget.date.month &&
            i.date.day == widget.date.day &&
            i.status != AvailabilityStatus.unknown)
        .toList()
      ..sort((a, b) => a.startTime.compareTo(b.startTime));

    final blocks = mergeAvailabilityInstances(dayInstances);

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
            Text(DateFormat('EEEE, MMMM d').format(widget.date), style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 16),
            if (blocks.isNotEmpty) ...[
              for (final block in blocks) ...[
                _BlockRow(block: block, onDelete: () => _clearBlock(block.start, block.end)),
                const SizedBox(height: 8),
              ],
              const SizedBox(height: 8),
            ],
            Text('Add availability', style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 10),
            Wrap(
              spacing: 8,
              children: [
                for (final status in const [AvailabilityStatus.available, AvailabilityStatus.maybe, AvailabilityStatus.busy])
                  ChoiceChip(
                    label: Text(status.label),
                    selected: _status == status,
                    onSelected: (_) => setState(() => _status = status),
                  ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: _TimeField(label: 'Start', value: _start.label, onTap: () => _pickTime(isStart: true)),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _TimeField(label: 'End', value: _end.label, onTap: () => _pickTime(isStart: false)),
                ),
              ],
            ),
            const SizedBox(height: 16),
            AppButton(label: 'Add', isLoading: _saving, onPressed: _addBlock),
          ],
        ),
      ),
    );
  }
}

class _BlockRow extends StatelessWidget {
  const _BlockRow({required this.block, required this.onDelete});

  final AvailabilityBlock block;
  final VoidCallback onDelete;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(14)),
      child: Row(
        children: [
          AvailabilityBadge(status: block.status, dense: true),
          const SizedBox(width: 10),
          Expanded(
            child: Text('${block.start.label} - ${block.end.label}', style: Theme.of(context).textTheme.titleMedium),
          ),
          IconButton(icon: const Icon(Icons.close_rounded, size: 20), onPressed: onDelete),
        ],
      ),
    );
  }
}

class _TimeField extends StatelessWidget {
  const _TimeField({required this.label, required this.value, required this.onTap});

  final String label;
  final String value;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
        decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(16)),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(label, style: const TextStyle(fontSize: 11, color: AppColors.unknownGray)),
            Text(value, style: const TextStyle(fontWeight: FontWeight.w600)),
          ],
        ),
      ),
    );
  }
}
