import '../../domain/entities/app_notification.dart';
import '../../domain/entities/notification_type.dart';
import '../../domain/repositories/notifications_repository.dart';
import '../datasources/notifications_remote_data_source.dart';
import '../dtos/notification_dto.dart';

class NotificationsRepositoryImpl implements NotificationsRepository {
  NotificationsRepositoryImpl(this._remoteDataSource);

  final NotificationsRemoteDataSource _remoteDataSource;

  @override
  Future<List<AppNotification>> getNotifications() async {
    final dtos = await _remoteDataSource.getNotifications();
    return dtos.map(_mapNotification).toList();
  }

  @override
  Future<void> markAsRead(String id) => _remoteDataSource.markAsRead(id);

  @override
  Future<void> markAllAsRead() => _remoteDataSource.markAllAsRead();

  @override
  Future<void> registerDeviceToken(String token, String platform) =>
      _remoteDataSource.registerDeviceToken(token, platform);

  @override
  Future<void> unregisterDeviceToken(String token) => _remoteDataSource.unregisterDeviceToken(token);

  AppNotification _mapNotification(NotificationDto dto) => AppNotification(
        id: dto.id,
        type: NotificationType.fromJson(dto.type),
        title: dto.title,
        body: dto.body,
        isRead: dto.isRead,
        createdAt: dto.createdAt,
      );
}
