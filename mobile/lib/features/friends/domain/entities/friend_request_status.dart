/// Mirrors the backend's `FriendRequestStatus` enum exactly (serialized as a JSON string).
enum FriendRequestStatus {
  pending,
  accepted,
  declined;

  static FriendRequestStatus fromJson(String value) => switch (value) {
        'Pending' => FriendRequestStatus.pending,
        'Accepted' => FriendRequestStatus.accepted,
        'Declined' => FriendRequestStatus.declined,
        _ => throw ArgumentError('Unknown FriendRequestStatus: $value'),
      };

  String toJson() => switch (this) {
        FriendRequestStatus.pending => 'Pending',
        FriendRequestStatus.accepted => 'Accepted',
        FriendRequestStatus.declined => 'Declined',
      };
}

/// Client-side only — picks which side of `GET /friends/requests?direction=` to query.
enum FriendRequestDirection {
  incoming,
  outgoing;

  String get queryValue => switch (this) {
        FriendRequestDirection.incoming => 'Incoming',
        FriendRequestDirection.outgoing => 'Outgoing',
      };
}
