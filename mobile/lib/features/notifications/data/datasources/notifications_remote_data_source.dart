import 'package:dio/dio.dart';

import '../dtos/notification_dto.dart';

class NotificationsRemoteDataSource {
  NotificationsRemoteDataSource(this._dio);

  final Dio _dio;

  Future<List<NotificationDto>> getNotifications() async {
    final response = await _dio.get('/notifications');
    return (response.data as List).map((e) => NotificationDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<void> markAsRead(String id) => _dio.post('/notifications/$id/read');

  Future<void> markAllAsRead() => _dio.post('/notifications/read-all');

  Future<void> registerDeviceToken(String token, String platform) =>
      _dio.post('/notifications/device-tokens', data: {'token': token, 'platform': platform});

  Future<void> unregisterDeviceToken(String token) =>
      _dio.delete('/notifications/device-tokens', queryParameters: {'token': token});
}
