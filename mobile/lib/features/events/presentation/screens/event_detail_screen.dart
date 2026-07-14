import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../../groups/presentation/providers/groups_providers.dart';
import '../../domain/entities/event_status.dart';
import '../providers/events_providers.dart';
import '../widgets/confirm_event_sheet.dart';
import '../widgets/event_location_option_tile.dart';
import '../widgets/event_time_option_tile.dart';

class EventDetailScreen extends ConsumerWidget {
  const EventDetailScreen({required this.eventId, super.key});

  final String eventId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final detailAsync = ref.watch(eventDetailProvider(eventId));
    final currentUserId = ref.watch(authControllerProvider).valueOrNull?.id;

    return Scaffold(
      body: detailAsync.when(
        loading: () => const SafeArea(
          child: Padding(padding: EdgeInsets.all(16), child: LoadingSkeleton(height: 200, borderRadius: 24)),
        ),
        error: (_, _) => const Center(child: Text("Couldn't load this event.")),
        data: (detail) {
          final event = detail.event;
          final membersAsync = ref.watch(groupMembersProvider(event.groupId));
          final members = membersAsync.valueOrNull ?? const [];
          final membersById = {for (final m in members) m.userId: m};
          final isOrganizer = event.createdByUserId == currentUserId;
          final myAttendance = detail.attendance.where((a) => a.userId == currentUserId).firstOrNull;

          final confirmedTime = event.confirmedTimeOptionId == null
              ? null
              : detail.timeOptions.where((t) => t.id == event.confirmedTimeOptionId).firstOrNull;
          final confirmedLocation = event.confirmedLocationOptionId == null
              ? null
              : detail.locationOptions.where((l) => l.id == event.confirmedLocationOptionId).firstOrNull;

          return CustomScrollView(
            slivers: [
              SliverAppBar(
                pinned: true,
                expandedHeight: 160,
                backgroundColor: AppColors.navy,
                foregroundColor: Colors.white,
                iconTheme: const IconThemeData(color: Colors.white),
                flexibleSpace: FlexibleSpaceBar(
                  titlePadding: const EdgeInsets.only(left: 16, bottom: 16, right: 60),
                  title: Text(event.title, style: const TextStyle(color: Colors.white, fontSize: 16)),
                  background: Container(
                    decoration: const BoxDecoration(gradient: AppColors.backgroundGradient),
                    child: Align(
                      alignment: Alignment.bottomRight,
                      child: Padding(
                        padding: const EdgeInsets.all(16),
                        child: _StatusPill(status: event.status),
                      ),
                    ),
                  ),
                ),
              ),
              SliverList(
                delegate: SliverChildListDelegate([
                  Padding(
                    padding: const EdgeInsets.all(16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (event.description != null && event.description!.isNotEmpty) ...[
                          Text(event.description!, style: Theme.of(context).textTheme.bodyLarge),
                          const SizedBox(height: 20),
                        ],
                        Row(
                          children: [
                            const Icon(Icons.schedule_rounded, color: AppColors.accentBlue, size: 20),
                            const SizedBox(width: 10),
                            Text(
                              confirmedTime != null
                                  ? DateFormat('EEEE, MMM d · h:mm a').format(confirmedTime.startUtc.toLocal())
                                  : 'Time not confirmed yet',
                              style: Theme.of(context).textTheme.bodyLarge,
                            ),
                          ],
                        ),
                        const SizedBox(height: 8),
                        Row(
                          children: [
                            const Icon(Icons.place_outlined, color: AppColors.accentBlue, size: 20),
                            const SizedBox(width: 10),
                            Expanded(
                              child: Text(
                                confirmedLocation?.name ?? 'Location not confirmed yet',
                                style: Theme.of(context).textTheme.bodyLarge,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 24),

                        Text('Your attendance', style: Theme.of(context).textTheme.titleMedium),
                        const SizedBox(height: 10),
                        Row(
                          children: [
                            for (final status in EventAttendanceStatus.values) ...[
                              Expanded(
                                child: Padding(
                                  padding: const EdgeInsets.only(right: 8),
                                  child: ChoiceChip(
                                    label: Text(status.label),
                                    selected: myAttendance?.status == status,
                                    onSelected: (_) =>
                                        ref.read(eventsRepositoryProvider).setAttendance(eventId, status).then(
                                              (_) => ref.read(eventDetailProvider(eventId).notifier).refresh(),
                                            ),
                                  ),
                                ),
                              ),
                            ],
                          ],
                        ),
                        const SizedBox(height: 24),

                        Row(
                          children: [
                            Expanded(child: Text('Reminder', style: Theme.of(context).textTheme.titleMedium)),
                            Switch(
                              value: true,
                              activeThumbColor: AppColors.navy,
                              onChanged: (_) => ScaffoldMessenger.of(context)
                                  .showSnackBar(const SnackBar(content: Text('Per-event reminders are coming soon.'))),
                            ),
                          ],
                        ),
                        const SizedBox(height: 16),

                        if (event.status != EventStatus.cancelled) ...[
                          Text('Time options', style: Theme.of(context).textTheme.titleMedium),
                          const SizedBox(height: 10),
                          for (final option in detail.timeOptions) ...[
                            EventTimeOptionTile(
                              option: option,
                              onToggleVote: () async {
                                final repo = ref.read(eventsRepositoryProvider);
                                if (option.hasCurrentUserVoted) {
                                  await repo.retractTimeVote(eventId, option.id);
                                } else {
                                  await repo.voteTimeOption(eventId, option.id);
                                }
                                ref.read(eventDetailProvider(eventId).notifier).refresh();
                              },
                            ),
                            const SizedBox(height: 8),
                          ],
                          const SizedBox(height: 16),
                          Text('Location options', style: Theme.of(context).textTheme.titleMedium),
                          const SizedBox(height: 10),
                          for (final option in detail.locationOptions) ...[
                            EventLocationOptionTile(
                              option: option,
                              onToggleVote: () async {
                                final repo = ref.read(eventsRepositoryProvider);
                                if (option.hasCurrentUserVoted) {
                                  await repo.retractLocationVote(eventId, option.id);
                                } else {
                                  await repo.voteLocationOption(eventId, option.id);
                                }
                                ref.read(eventDetailProvider(eventId).notifier).refresh();
                              },
                            ),
                            const SizedBox(height: 8),
                          ],
                        ],
                        const SizedBox(height: 24),

                        Text('Attendees (${detail.attendance.length})', style: Theme.of(context).textTheme.titleMedium),
                        const SizedBox(height: 10),
                        Wrap(
                          spacing: 10,
                          runSpacing: 10,
                          children: [
                            for (final attendance in detail.attendance)
                              if (membersById[attendance.userId] != null)
                                Column(
                                  children: [
                                    ProfileAvatar(
                                      displayName: membersById[attendance.userId]!.displayName,
                                      imageUrl: membersById[attendance.userId]!.profilePictureUrl,
                                      size: 44,
                                    ),
                                    const SizedBox(height: 4),
                                    Text(attendance.status.label, style: const TextStyle(fontSize: 10)),
                                  ],
                                ),
                          ],
                        ),

                        if (isOrganizer && event.status == EventStatus.proposed) ...[
                          const SizedBox(height: 28),
                          AppButton(
                            label: 'Confirm event',
                            icon: Icons.check_circle_outline_rounded,
                            onPressed: detail.timeOptions.isEmpty || detail.locationOptions.isEmpty
                                ? null
                                : () => showModalBottomSheet(
                                      context: context,
                                      isScrollControlled: true,
                                      backgroundColor: Colors.white,
                                      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
                                      builder: (context) => ConfirmEventSheet(
                                        timeOptions: detail.timeOptions,
                                        locationOptions: detail.locationOptions,
                                        onConfirm: (timeId, locationId) async {
                                          await ref
                                              .read(eventsRepositoryProvider)
                                              .confirmEvent(eventId, timeOptionId: timeId, locationOptionId: locationId);
                                          if (context.mounted) Navigator.of(context).pop();
                                          ref.read(eventDetailProvider(eventId).notifier).refresh();
                                        },
                                      ),
                                    ),
                          ),
                        ],
                        if (isOrganizer && event.status != EventStatus.cancelled) ...[
                          const SizedBox(height: 10),
                          AppButton(
                            label: 'Cancel event',
                            variant: AppButtonVariant.text,
                            onPressed: () async {
                              await ref.read(eventsRepositoryProvider).cancelEvent(eventId);
                              if (context.mounted) context.pop();
                            },
                          ),
                        ],
                      ],
                    ),
                  ),
                ]),
              ),
            ],
          );
        },
      ),
    );
  }
}

class _StatusPill extends StatelessWidget {
  const _StatusPill({required this.status});

  final EventStatus status;

  @override
  Widget build(BuildContext context) {
    final (label, color) = switch (status) {
      EventStatus.proposed => ('Voting in progress', AppColors.gold),
      EventStatus.confirmed => ('Confirmed', AppColors.availableGreen),
      EventStatus.cancelled => ('Cancelled', AppColors.busyRed),
    };

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(999)),
      child: Text(label, style: TextStyle(color: color, fontWeight: FontWeight.w700, fontSize: 12)),
    );
  }
}
