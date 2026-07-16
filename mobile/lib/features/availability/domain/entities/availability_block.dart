import '../../../../core/models/availability_status.dart';
import '../../../../core/models/time_slot.dart';
import 'availability_instance.dart';

/// A run of contiguous same-status `AvailabilityInstance` slots, collapsed into one displayable
/// range (e.g. "9:00 AM - 5:00 PM") instead of sixteen separate 30-minute rows.
class AvailabilityBlock {
  const AvailabilityBlock({required this.start, required this.end, required this.status});

  final TimeSlot start;
  final TimeSlot end;
  final AvailabilityStatus status;
}

/// [instances] must already be sorted by `startTime` and scoped to a single day.
List<AvailabilityBlock> mergeAvailabilityInstances(List<AvailabilityInstance> instances) {
  final blocks = <AvailabilityBlock>[];
  for (final instance in instances) {
    final last = blocks.isEmpty ? null : blocks.last;
    if (last != null && last.end == instance.startTime && last.status == instance.status) {
      blocks[blocks.length - 1] = AvailabilityBlock(start: last.start, end: instance.startTime.next, status: last.status);
    } else {
      blocks.add(AvailabilityBlock(start: instance.startTime, end: instance.startTime.next, status: instance.status));
    }
  }
  return blocks;
}
