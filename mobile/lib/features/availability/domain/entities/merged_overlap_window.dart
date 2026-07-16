import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/models/time_slot.dart';
import 'overlap_window.dart';

part 'merged_overlap_window.freezed.dart';

/// A run of contiguous 30-minute `OverlapWindow`s on the same date with the exact same set of
/// available/maybe members, collapsed into one displayable range (e.g. "2:00 PM - 4:00 PM")
/// instead of four separate 30-minute cards that all say the same thing.
@freezed
abstract class MergedOverlapWindow with _$MergedOverlapWindow {
  const factory MergedOverlapWindow({
    required DateTime date,
    required TimeSlot startTime,
    required TimeSlot endTime,
    required List<String> availableUserIds,
    required int availableCount,
    required int totalMembers,
    required List<String> maybeUserIds,
  }) = _MergedOverlapWindow;
}

bool _sameUserSet(List<String> a, List<String> b) => a.length == b.length && a.toSet().containsAll(b);

/// Merges same-date, contiguous, identical-availability windows. Input does not need to be
/// pre-sorted; output is sorted by date then startTime.
List<MergedOverlapWindow> mergeOverlapWindows(List<OverlapWindow> windows) {
  final sorted = [...windows]..sort((a, b) {
      final byDate = a.date.compareTo(b.date);
      return byDate != 0 ? byDate : a.startTime.compareTo(b.startTime);
    });

  final merged = <MergedOverlapWindow>[];
  for (final window in sorted) {
    final last = merged.isEmpty ? null : merged.last;
    final isContinuation = last != null &&
        _sameDay(last.date, window.date) &&
        last.endTime == window.startTime &&
        _sameUserSet(last.availableUserIds, window.availableUserIds) &&
        _sameUserSet(last.maybeUserIds, window.maybeUserIds);

    if (isContinuation) {
      merged[merged.length - 1] = last.copyWith(endTime: window.startTime.next);
    } else {
      merged.add(MergedOverlapWindow(
        date: window.date,
        startTime: window.startTime,
        endTime: window.startTime.next,
        availableUserIds: window.availableUserIds,
        availableCount: window.availableCount,
        totalMembers: window.totalMembers,
        maybeUserIds: window.maybeUserIds,
      ));
    }
  }
  return merged;
}

bool _sameDay(DateTime a, DateTime b) => a.year == b.year && a.month == b.month && a.day == b.day;
