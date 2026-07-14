import '../entities/app_notification.dart';

/// Shaped to match the intended real endpoints (`GET /notifications`, `PATCH
/// /notifications/{id}`, `POST /notifications/read-all`) even though no backend exists yet —
/// swapping the fake implementation for a real Dio-backed one later is a one-file change.
abstract class NotificationsRepository {
  Future<List<AppNotification>> getNotifications();

  Future<void> markAsRead(String id);

  Future<void> markAllAsRead();
}
