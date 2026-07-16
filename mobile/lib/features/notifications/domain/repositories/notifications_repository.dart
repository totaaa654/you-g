import '../entities/app_notification.dart';

abstract class NotificationsRepository {
  Future<List<AppNotification>> getNotifications();

  Future<void> markAsRead(String id);

  Future<void> markAllAsRead();

  Future<void> registerDeviceToken(String token, String platform);

  Future<void> unregisterDeviceToken(String token);
}
