import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../availability/domain/entities/overlap_window.dart';
import '../../../availability/presentation/providers/availability_providers.dart';
import '../../../events/domain/entities/event.dart';
import '../../../events/domain/entities/event_status.dart';
import '../../../events/presentation/providers/events_providers.dart';
import '../../../groups/domain/entities/group.dart';
import '../../../groups/presentation/providers/groups_providers.dart';

/// A confirmed event resolved with its actual scheduled time — the plain `Event`/`EventDto`
/// list endpoints don't carry a start time (that lives on the confirmed `EventTimeOption`),
/// so this is only knowable after fetching each event's detail.
class ScheduledEvent {
  const ScheduledEvent({required this.event, required this.groupName, required this.startUtc});

  final Event event;
  final String groupName;
  final DateTime startUtc;
}

/// Confirmed events across every group the user belongs to, resolved to their real start
/// time and sorted soonest-first. Proposed (not-yet-confirmed) events are deliberately
/// excluded here — without a confirmed time they have nothing to sort by, so they don't
/// belong in an "upcoming" list; they're still visible from each Group's own event list.
final upcomingScheduledEventsProvider = FutureProvider.autoDispose<List<ScheduledEvent>>((ref) async {
  final groups = await ref.watch(myGroupsProvider.future);
  final eventsRepository = ref.watch(eventsRepositoryProvider);
  final groupsById = {for (final g in groups) g.id: g};

  final perGroupEvents = await Future.wait(groups.map((g) => eventsRepository.getGroupEvents(g.id)));
  final confirmed = perGroupEvents.expand((events) => events).where((e) => e.status == EventStatus.confirmed).toList();

  final details = await Future.wait(confirmed.map((e) => eventsRepository.getEventById(e.id)));

  final scheduled = <ScheduledEvent>[];
  for (final detail in details) {
    final timeOptionId = detail.event.confirmedTimeOptionId;
    if (timeOptionId == null) continue;
    final timeOption = detail.timeOptions.where((t) => t.id == timeOptionId).firstOrNull;
    if (timeOption == null) continue;
    scheduled.add(ScheduledEvent(
      event: detail.event,
      groupName: groupsById[detail.event.groupId]?.name ?? 'Group',
      startUtc: timeOption.startUtc,
    ));
  }

  final now = DateTime.now().toUtc();
  final upcoming = scheduled.where((s) => s.startUtc.isAfter(now)).toList()
    ..sort((a, b) => a.startUtc.compareTo(b.startUtc));
  return upcoming;
});

/// The suggested-meetup card needs *a* group to compute overlap for. There's no natural
/// "primary group" concept in the backend, so this just picks the first group returned by
/// `GET /groups` — reasonable for a user with a handful of groups, which is the expected
/// scale here.
final suggestedMeetupProvider = FutureProvider.autoDispose<(Group, OverlapWindow)?>((ref) async {
  final groups = await ref.watch(myGroupsProvider.future);
  if (groups.isEmpty) return null;

  final group = groups.first;
  final now = DateTime.now();
  final today = DateTime(now.year, now.month, now.day);
  final windows = await ref.watch(availabilityRepositoryProvider).getGroupOverlap(
        group.id,
        from: today,
        to: today.add(const Duration(days: 7)),
      );
  if (windows.isEmpty) return null;

  final best = [...windows]..sort((a, b) => b.availableCount.compareTo(a.availableCount));
  return (group, best.first);
});
