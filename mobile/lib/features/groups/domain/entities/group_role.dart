/// Mirrors the backend's `GroupRole` enum exactly (serialized as a JSON string).
enum GroupRole {
  member,
  admin;

  static GroupRole fromJson(String value) => switch (value) {
        'Member' => GroupRole.member,
        'Admin' => GroupRole.admin,
        _ => throw ArgumentError('Unknown GroupRole: $value'),
      };

  String toJson() => switch (this) {
        GroupRole.member => 'Member',
        GroupRole.admin => 'Admin',
      };
}
