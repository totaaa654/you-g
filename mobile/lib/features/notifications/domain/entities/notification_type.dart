/// Mirrors the backend's `NotificationType` enum exactly (serialized as a JSON string).
enum NotificationType {
  friendRequest,
  groupInvite,
  eventReminder,
  scheduleUpdate;

  static NotificationType fromJson(String value) => switch (value) {
        'FriendRequest' => NotificationType.friendRequest,
        'GroupInvite' => NotificationType.groupInvite,
        'EventReminder' => NotificationType.eventReminder,
        'ScheduleUpdate' => NotificationType.scheduleUpdate,
        _ => throw ArgumentError('Unknown NotificationType: $value'),
      };

  String toJson() => switch (this) {
        NotificationType.friendRequest => 'FriendRequest',
        NotificationType.groupInvite => 'GroupInvite',
        NotificationType.eventReminder => 'EventReminder',
        NotificationType.scheduleUpdate => 'ScheduleUpdate',
      };
}
