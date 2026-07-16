/// The two outcomes an admin can choose for a pending `GroupJoinRequest` — mirrors the
/// subset of the backend's `GroupJoinRequestStatus` enum this app ever sends.
enum GroupJoinRequestResponse {
  accepted,
  declined;

  String toJson() => switch (this) {
        GroupJoinRequestResponse.accepted => 'Accepted',
        GroupJoinRequestResponse.declined => 'Declined',
      };
}
