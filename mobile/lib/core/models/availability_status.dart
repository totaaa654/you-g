/// Mirrors the backend's `AvailabilityStatus` enum exactly (serialized as a JSON string).
/// Shared across Calendar, Smart Time Finder, and Home — not owned by a single feature.
enum AvailabilityStatus {
  available,
  busy,
  maybe,
  unknown;

  static AvailabilityStatus fromJson(String value) => switch (value) {
        'Available' => AvailabilityStatus.available,
        'Busy' => AvailabilityStatus.busy,
        'Maybe' => AvailabilityStatus.maybe,
        'Unknown' => AvailabilityStatus.unknown,
        _ => throw ArgumentError('Unknown AvailabilityStatus: $value'),
      };

  String toJson() => switch (this) {
        AvailabilityStatus.available => 'Available',
        AvailabilityStatus.busy => 'Busy',
        AvailabilityStatus.maybe => 'Maybe',
        AvailabilityStatus.unknown => 'Unknown',
      };

  String get label => switch (this) {
        AvailabilityStatus.available => 'Available',
        AvailabilityStatus.busy => 'Busy',
        AvailabilityStatus.maybe => 'Maybe',
        AvailabilityStatus.unknown => 'Unknown',
      };
}

/// Mirrors the backend's `Daypart` enum exactly.
enum Daypart {
  morning,
  afternoon,
  evening,
  night,
  wholeDay;

  static Daypart fromJson(String value) => switch (value) {
        'Morning' => Daypart.morning,
        'Afternoon' => Daypart.afternoon,
        'Evening' => Daypart.evening,
        'Night' => Daypart.night,
        'WholeDay' => Daypart.wholeDay,
        _ => throw ArgumentError('Unknown Daypart: $value'),
      };

  String toJson() => switch (this) {
        Daypart.morning => 'Morning',
        Daypart.afternoon => 'Afternoon',
        Daypart.evening => 'Evening',
        Daypart.night => 'Night',
        Daypart.wholeDay => 'WholeDay',
      };

  String get label => switch (this) {
        Daypart.morning => 'Morning',
        Daypart.afternoon => 'Afternoon',
        Daypart.evening => 'Evening',
        Daypart.night => 'Night',
        Daypart.wholeDay => 'All day',
      };
}
