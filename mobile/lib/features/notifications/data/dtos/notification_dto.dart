import 'package:freezed_annotation/freezed_annotation.dart';

part 'notification_dto.freezed.dart';
part 'notification_dto.g.dart';

/// Mirrors backend `NotificationDto` exactly. `type` stays a raw wire string here
/// ("FriendRequest"/"GroupInvite"/...) — the repository maps it to `NotificationType`.
@freezed
abstract class NotificationDto with _$NotificationDto {
  const factory NotificationDto({
    required String id,
    required String type,
    required String title,
    required String body,
    required String payload,
    required bool isRead,
    required DateTime createdAt,
  }) = _NotificationDto;

  factory NotificationDto.fromJson(Map<String, dynamic> json) => _$NotificationDtoFromJson(json);
}
