import 'package:freezed_annotation/freezed_annotation.dart';

import 'notification_type.dart';

part 'app_notification.freezed.dart';

/// Named `AppNotification`, not `Notification` — that name collides with Dart's own
/// `dart:core` `Notification` widget-tree class.
@freezed
abstract class AppNotification with _$AppNotification {
  const factory AppNotification({
    required String id,
    required NotificationType type,
    required String title,
    required String body,
    required bool isRead,
    required DateTime createdAt,
  }) = _AppNotification;
}
