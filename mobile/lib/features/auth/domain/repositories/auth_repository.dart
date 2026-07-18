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

  /// Always succeeds from the caller's perspective whether or not the email is registered —
  /// the backend deliberately doesn't reveal which, to avoid account enumeration.
  Future<void> forgotPassword(String email);

  Future<void> resetPassword({required String email, required String code, required String newPassword});
}
