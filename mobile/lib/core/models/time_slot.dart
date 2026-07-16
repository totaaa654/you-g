/// A 30-minute-aligned point in time-of-day, with no date component — mirrors the backend's
/// `TimeOnly StartTime` on `AvailabilityInstance`/`AvailabilityRule` (each row covers this time
/// up to this time + 30 minutes). Replaced the old fixed `Daypart` enum.
class TimeSlot implements Comparable<TimeSlot> {
  const TimeSlot(this.minutesSinceMidnight);

  factory TimeSlot.fromJson(String value) {
    final parts = value.split(':');
    return TimeSlot(int.parse(parts[0]) * 60 + int.parse(parts[1]));
  }

  /// Rounds down to the nearest 30-minute boundary — used to snap a raw `TimeOfDay` picker
  /// result onto the grid the backend actually stores.
  factory TimeSlot.fromMinutes(int hour, int minute) {
    final total = hour * 60 + minute;
    return TimeSlot((total ~/ 30) * 30);
  }

  final int minutesSinceMidnight;

  static const slotsPerDay = 48;

  int get hour => (minutesSinceMidnight ~/ 60) % 24;
  int get minute => minutesSinceMidnight % 60;

  TimeSlot get next => TimeSlot(minutesSinceMidnight + 30);

  String toJson() => '${hour.toString().padLeft(2, '0')}:${minute.toString().padLeft(2, '0')}:00';

  String get label {
    final h12 = hour % 12 == 0 ? 12 : hour % 12;
    final period = hour < 12 ? 'AM' : 'PM';
    final mm = minute.toString().padLeft(2, '0');
    return '$h12:$mm $period';
  }

  @override
  int compareTo(TimeSlot other) => minutesSinceMidnight.compareTo(other.minutesSinceMidnight);

  @override
  bool operator ==(Object other) => other is TimeSlot && other.minutesSinceMidnight == minutesSinceMidnight;

  @override
  int get hashCode => minutesSinceMidnight.hashCode;

  @override
  String toString() => label;
}
