import '../../domain/entities/app_notification.dart';
import '../../domain/entities/notification_type.dart';
import '../../domain/repositories/notifications_repository.dart';

/// In-memory only — there is no Notifications backend yet (the domain entity/DB table exist,
/// but no controller was ever built). Illustrative data, same honesty bar as Home's Friends
/// Activity: never presented as real, never persisted beyond this session.
class FakeNotificationsRepository implements NotificationsRepository {
  FakeNotificationsRepository() {
    final now = DateTime.now();
    _notifications = [
      AppNotification(
        id: '1',
        type: NotificationType.friendRequest,
        title: 'New friend request',
        body: 'Maya wants to be your friend.',
        isRead: false,
        createdAt: now.subtract(const Duration(hours: 2)),
      ),
      AppNotification(
        id: '2',
        type: NotificationType.eventReminder,
        title: 'Event starting soon',
        body: 'Beach Day starts in 3 hours.',
        isRead: false,
        createdAt: now.subtract(const Duration(hours: 5)),
      ),
      AppNotification(
        id: '3',
        type: NotificationType.groupInvite,
        title: 'Group invite',
        body: "You've been added to Hiking Crew.",
        isRead: true,
        createdAt: now.subtract(const Duration(days: 1, hours: 3)),
      ),
      AppNotification(
        id: '4',
        type: NotificationType.scheduleUpdate,
        title: 'Event time changed',
        body: 'Movie Night moved to Saturday, 7:00 PM.',
        isRead: true,
        createdAt: now.subtract(const Duration(days: 1, hours: 9)),
      ),
      AppNotification(
        id: '5',
        type: NotificationType.friendRequest,
        title: 'Friend request accepted',
        body: 'Jordan accepted your friend request.',
        isRead: true,
        createdAt: now.subtract(const Duration(days: 4)),
      ),
    ];
  }

  late final List<AppNotification> _notifications;

  @override
  Future<List<AppNotification>> getNotifications() async =>
      [..._notifications]..sort((a, b) => b.createdAt.compareTo(a.createdAt));

  @override
  Future<void> markAsRead(String id) async {
    final index = _notifications.indexWhere((n) => n.id == id);
    if (index != -1) _notifications[index] = _notifications[index].copyWith(isRead: true);
  }

  @override
  Future<void> markAllAsRead() async {
    for (var i = 0; i < _notifications.length; i++) {
      _notifications[i] = _notifications[i].copyWith(isRead: true);
    }
  }
}
