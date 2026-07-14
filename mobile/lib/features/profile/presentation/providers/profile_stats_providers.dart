import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/models/availability_status.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../../availability/presentation/providers/availability_providers.dart';
import '../../../events/domain/entities/event_status.dart';
import '../../../events/presentation/providers/events_providers.dart';
import '../../../groups/presentation/providers/groups_providers.dart';

/// Count of events (across every group the user belongs to) where they're marked Going.
/// Computed client-side from the list+detail endpoints — there's no dedicated backend
/// aggregate for this, so it costs one request per event. Fine at this app's scale; a real
/// aggregate endpoint would be the fix if that ever stopped being true.
final eventsJoinedCountProvider = FutureProvider.autoDispose<int>((ref) async {
  final userId = ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
  if (userId == null) return 0;

  final groups = await ref.watch(myGroupsProvider.future);
  final eventsRepository = ref.watch(eventsRepositoryProvider);

  final perGroupEvents = await Future.wait(groups.map((g) => eventsRepository.getGroupEvents(g.id)));
  final allEvents = perGroupEvents.expand((e) => e).where((e) => e.status != EventStatus.cancelled).toList();

  final details = await Future.wait(allEvents.map((e) => eventsRepository.getEventById(e.id)));
  return details
      .where((d) => d.attendance.any((a) => a.userId == userId && a.status == EventAttendanceStatus.going))
      .length;
});

/// Available dayparts / total dayparts set, over the next 7 days — a real, derived metric
/// rather than a fabricated score (the backend has no such concept).
final availabilityScoreProvider = FutureProvider.autoDispose<double?>((ref) async {
  ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
  final now = DateTime.now();
  final today = DateTime(now.year, now.month, now.day);
  final instances = await ref.watch(availabilityRepositoryProvider).getMyInstances(
        from: today,
        to: today.add(const Duration(days: 6)),
      );
  if (instances.isEmpty) return null;

  final available = instances.where((i) => i.status == AvailabilityStatus.available).length;
  return available / instances.length;
});
