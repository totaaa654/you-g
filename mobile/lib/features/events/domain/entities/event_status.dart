/// Mirrors the backend's `EventStatus` enum exactly (serialized as a JSON string).
enum EventStatus {
  proposed,
  confirmed,
  cancelled;

  static EventStatus fromJson(String value) => switch (value) {
        'Proposed' => EventStatus.proposed,
        'Confirmed' => EventStatus.confirmed,
        'Cancelled' => EventStatus.cancelled,
        _ => throw ArgumentError('Unknown EventStatus: $value'),
      };

  String toJson() => switch (this) {
        EventStatus.proposed => 'Proposed',
        EventStatus.confirmed => 'Confirmed',
        EventStatus.cancelled => 'Cancelled',
      };
}

/// Mirrors the backend's `EventAttendanceStatus` enum exactly.
enum EventAttendanceStatus {
  going,
  maybe,
  cantGo;

  static EventAttendanceStatus fromJson(String value) => switch (value) {
        'Going' => EventAttendanceStatus.going,
        'Maybe' => EventAttendanceStatus.maybe,
        'CantGo' => EventAttendanceStatus.cantGo,
        _ => throw ArgumentError('Unknown EventAttendanceStatus: $value'),
      };

  String toJson() => switch (this) {
        EventAttendanceStatus.going => 'Going',
        EventAttendanceStatus.maybe => 'Maybe',
        EventAttendanceStatus.cantGo => 'CantGo',
      };

  String get label => switch (this) {
        EventAttendanceStatus.going => 'Going',
        EventAttendanceStatus.maybe => 'Maybe',
        EventAttendanceStatus.cantGo => "Can't go",
      };
}
