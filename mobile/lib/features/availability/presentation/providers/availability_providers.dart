import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/models/time_slot.dart';
import '../../../../core/network/network_providers.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../../home/presentation/providers/home_providers.dart';
import '../../data/datasources/availability_remote_data_source.dart';
import '../../data/repositories/availability_repository_impl.dart';
import '../../domain/entities/availability_instance.dart';
import '../../domain/entities/overlap_window.dart';
import '../../domain/repositories/availability_repository.dart';

final availabilityRemoteDataSourceProvider =
    Provider<AvailabilityRemoteDataSource>((ref) => AvailabilityRemoteDataSource(ref.watch(dioProvider)));

final availabilityRepositoryProvider = Provider<AvailabilityRepository>(
  (ref) => AvailabilityRepositoryImpl(ref.watch(availabilityRemoteDataSourceProvider)),
);

/// Keyed by a `(from, to)` window rather than watched implicitly, since Calendar needs to
/// page between months/weeks and Home needs just "today" — one shared notifier per range.
final myInstancesProvider =
    AsyncNotifierProvider.family<MyInstancesNotifier, List<AvailabilityInstance>, DateRangeKey>(
  MyInstancesNotifier.new,
);

class DateRangeKey {
  const DateRangeKey(this.from, this.to);

  final DateTime from;
  final DateTime to;

  @override
  bool operator ==(Object other) =>
      other is DateRangeKey && other.from == from && other.to == to;

  @override
  int get hashCode => Object.hash(from, to);
}

class MyInstancesNotifier extends FamilyAsyncNotifier<List<AvailabilityInstance>, DateRangeKey> {
  @override
  Future<List<AvailabilityInstance>> build(DateRangeKey arg) {
    // Re-runs on login/logout/account switch — the family key is only a date range, not the
    // user, so without this the cache would otherwise leak the previous account's availability
    // for that same range. See the identical comment in `MyGroupsNotifier.build()`.
    ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
    return ref.watch(availabilityRepositoryProvider).getMyInstances(from: arg.from, to: arg.to);
  }

  Future<void> setStatus(DateTime date, TimeSlot startTime, AvailabilityStatus status) =>
      setStatusRange(date, startTime, startTime.next, status);

  /// Sets every 30-minute slot from [startTime] (inclusive) to [endTime] (exclusive) to
  /// [status] in one batch upsert — what the day editor's "add availability" range maps onto.
  Future<void> setStatusRange(DateTime date, TimeSlot startTime, TimeSlot endTime, AvailabilityStatus status) async {
    final repository = ref.read(availabilityRepositoryProvider);
    final slots = <TimeSlot>[];
    for (var t = startTime; t.minutesSinceMidnight < endTime.minutesSinceMidnight; t = t.next) {
      slots.add(t);
    }
    final updated = [for (final slot in slots) AvailabilityInstance(date: date, startTime: slot, status: status)];
    await repository.upsertInstances(updated);

    state = state.whenData((instances) {
      final withoutThese = instances.where((i) => !(_sameDay(i.date, date) && slots.contains(i.startTime))).toList();
      return [...withoutThese, ...updated];
    });

    // Both are computed from availability, but neither watches this notifier directly, so
    // they'd otherwise keep showing stale overlap/suggestions after you update your own
    // availability until you leave and come back to those screens.
    ref.invalidate(suggestedMeetupProvider);
    ref.invalidate(groupOverlapProvider);
  }
}

bool _sameDay(DateTime a, DateTime b) => a.year == b.year && a.month == b.month && a.day == b.day;

final groupOverlapProvider = FutureProvider.family.autoDispose<List<OverlapWindow>, GroupOverlapQuery>(
  (ref, query) => ref.watch(availabilityRepositoryProvider).getGroupOverlap(
        query.groupId,
        from: query.from,
        to: query.to,
        weekendOnly: query.weekendOnly,
      ),
);

class GroupOverlapQuery {
  const GroupOverlapQuery({
    required this.groupId,
    required this.from,
    required this.to,
    this.weekendOnly = false,
  });

  final String groupId;
  final DateTime from;
  final DateTime to;
  final bool weekendOnly;

  @override
  bool operator ==(Object other) =>
      other is GroupOverlapQuery &&
      other.groupId == groupId &&
      other.from == from &&
      other.to == to &&
      other.weekendOnly == weekendOnly;

  @override
  int get hashCode => Object.hash(groupId, from, to, weekendOnly);
}
