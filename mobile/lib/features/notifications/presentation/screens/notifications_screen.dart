import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/widgets/empty_state.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../domain/entities/app_notification.dart';
import '../providers/notifications_providers.dart';
import '../widgets/notification_tile.dart';

class NotificationsScreen extends ConsumerWidget {
  const NotificationsScreen({super.key});

  static Map<String, List<AppNotification>> _grouped(List<AppNotification> notifications) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final yesterday = today.subtract(const Duration(days: 1));

    final groups = <String, List<AppNotification>>{'Today': [], 'Yesterday': [], 'Earlier': []};
    for (final n in notifications) {
      final day = DateTime(n.createdAt.year, n.createdAt.month, n.createdAt.day);
      if (day == today) {
        groups['Today']!.add(n);
      } else if (day == yesterday) {
        groups['Yesterday']!.add(n);
      } else {
        groups['Earlier']!.add(n);
      }
    }
    groups.removeWhere((_, items) => items.isEmpty);
    return groups;
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final notificationsAsync = ref.watch(notificationsListProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Notifications'),
        actions: [
          TextButton(
            onPressed: () => ref.read(notificationsListProvider.notifier).markAllAsRead(),
            child: const Text('Mark all read'),
          ),
        ],
      ),
      body: notificationsAsync.when(
        loading: () => ListView(
          padding: const EdgeInsets.all(16),
          children: const [
            LoadingSkeleton(height: 72, borderRadius: 20),
            SizedBox(height: 12),
            LoadingSkeleton(height: 72, borderRadius: 20),
          ],
        ),
        error: (_, _) =>
            const EmptyState(icon: Icons.error_outline_rounded, title: "Couldn't load notifications"),
        data: (notifications) {
          if (notifications.isEmpty) {
            return const EmptyState(
              icon: Icons.notifications_none_rounded,
              title: "You're all caught up",
              message: 'New notifications will show up here.',
            );
          }

          final groups = _grouped(notifications);

          return ListView(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
            children: [
              for (final entry in groups.entries) ...[
                Padding(
                  padding: const EdgeInsets.symmetric(vertical: 10),
                  child: Text(entry.key, style: Theme.of(context).textTheme.titleMedium),
                ),
                for (final notification in entry.value) ...[
                  NotificationTile(
                    notification: notification,
                    onTap: () => ref.read(notificationsListProvider.notifier).markAsRead(notification.id),
                  ),
                  const SizedBox(height: 10),
                ],
              ],
            ],
          );
        },
      ),
    );
  }
}
