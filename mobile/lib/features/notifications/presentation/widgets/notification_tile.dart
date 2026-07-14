import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../domain/entities/app_notification.dart';
import '../../domain/entities/notification_type.dart';

(IconData, Color) _iconFor(NotificationType type) => switch (type) {
      NotificationType.friendRequest => (Icons.person_add_alt_1_rounded, AppColors.accentBlue),
      NotificationType.groupInvite => (Icons.groups_rounded, AppColors.navy),
      NotificationType.eventReminder => (Icons.event_rounded, AppColors.gold),
      NotificationType.scheduleUpdate => (Icons.update_rounded, AppColors.availableGreen),
    };

class NotificationTile extends StatelessWidget {
  const NotificationTile({required this.notification, required this.onTap, super.key});

  final AppNotification notification;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final (icon, color) = _iconFor(notification.type);

    return AppCard(
      onTap: onTap,
      padding: const EdgeInsets.all(14),
      color: notification.isRead ? Colors.white : AppColors.fog,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(color: color.withValues(alpha: 0.12), borderRadius: BorderRadius.circular(12)),
            child: Icon(icon, color: color, size: 20),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(notification.title, style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 2),
                Text(notification.body, style: Theme.of(context).textTheme.bodyMedium),
                const SizedBox(height: 4),
                Text(
                  DateFormat('h:mm a').format(notification.createdAt),
                  style: const TextStyle(fontSize: 11, color: AppColors.unknownGray),
                ),
              ],
            ),
          ),
          if (!notification.isRead)
            Container(width: 8, height: 8, margin: const EdgeInsets.only(top: 4), decoration: const BoxDecoration(color: AppColors.navy, shape: BoxShape.circle)),
        ],
      ),
    );
  }
}
