/// Mirrors the backend's `NotificationType` enum exactly — kept consistent even though
/// there's no Notifications backend yet, so wiring the real API later is a data-source swap,
/// not a redesign.
enum NotificationType {
  friendRequest,
  groupInvite,
  eventReminder,
  scheduleUpdate,
}
