/// Mirrors the backend's `ThemeMode` enum exactly (serialized as a JSON string). Named
/// `ThemePreference` to avoid clashing with Flutter's own `ThemeMode`.
enum ThemePreference {
  system,
  light,
  dark;

  static ThemePreference fromJson(String value) => switch (value) {
        'System' => ThemePreference.system,
        'Light' => ThemePreference.light,
        'Dark' => ThemePreference.dark,
        _ => throw ArgumentError('Unknown ThemeMode: $value'),
      };

  String toJson() => switch (this) {
        ThemePreference.system => 'System',
        ThemePreference.light => 'Light',
        ThemePreference.dark => 'Dark',
      };

  String get label => switch (this) {
        ThemePreference.system => 'System',
        ThemePreference.light => 'Light',
        ThemePreference.dark => 'Dark',
      };
}
