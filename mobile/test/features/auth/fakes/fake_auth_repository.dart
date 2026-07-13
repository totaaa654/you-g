import 'package:dio/dio.dart';
import 'package:youg/features/auth/domain/entities/user_summary.dart';
import 'package:youg/features/auth/domain/repositories/auth_repository.dart';

/// Hand-written fake, mirroring the backend's testing convention of in-memory fakes
/// over a mocking library (docs/02-ARCHITECTURE.md / backend Application.Tests/Fakes).
class FakeAuthRepository implements AuthRepository {
  UserSummary? restoredUser;
  bool throwOnLogin = false;
  bool throwOnRegister = false;

  UserSummary? loggedInUser;
  bool logoutCalled = false;

  static const _testUser = UserSummary(id: '1', username: 'mayag', displayName: 'Maya', friendCode: 'YG-000000');

  @override
  Future<UserSummary?> restoreSession() async => restoredUser;

  @override
  Future<UserSummary> login({required String email, required String password}) async {
    if (throwOnLogin) {
      throw DioException(
        requestOptions: RequestOptions(path: '/auth/login'),
        response: Response(
          requestOptions: RequestOptions(path: '/auth/login'),
          statusCode: 401,
          data: {'title': 'Invalid email or password.'},
        ),
      );
    }

    loggedInUser = _testUser;
    return _testUser;
  }

  @override
  Future<UserSummary> register({
    required String email,
    required String password,
    required String username,
    required String displayName,
    required String timeZoneId,
  }) async {
    if (throwOnRegister) {
      throw DioException(
        requestOptions: RequestOptions(path: '/auth/register'),
        response: Response(
          requestOptions: RequestOptions(path: '/auth/register'),
          statusCode: 409,
          data: {'title': 'An account with this email already exists.'},
        ),
      );
    }

    loggedInUser = _testUser;
    return _testUser;
  }

  @override
  Future<void> logout() async {
    logoutCalled = true;
    loggedInUser = null;
  }
}
