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
