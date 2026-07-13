import '../entities/user_summary.dart';

abstract class AuthRepository {
  Future<UserSummary> register({
    required String email,
    required String password,
    required String username,
    required String displayName,
    required String timeZoneId,
  });

  Future<UserSummary> login({required String email, required String password});

  Future<void> logout();

  /// Restores the cached user for a locally-stored session, or null if there isn't one.
  /// Doesn't guarantee the tokens are still valid server-side — the Dio refresh
  /// interceptor handles that on the first real request.
  Future<UserSummary?> restoreSession();
}
