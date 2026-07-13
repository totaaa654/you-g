import '../../../../core/storage/token_storage.dart';
import '../../domain/entities/user_summary.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_data_source.dart';
import '../datasources/cached_user_storage.dart';
import '../dtos/auth_result_dto.dart';

class AuthRepositoryImpl implements AuthRepository {
  AuthRepositoryImpl(this._remoteDataSource, this._tokenStorage, this._cachedUserStorage);

  final AuthRemoteDataSource _remoteDataSource;
  final TokenStorage _tokenStorage;
  final CachedUserStorage _cachedUserStorage;

  @override
  Future<UserSummary> register({
    required String email,
    required String password,
    required String username,
    required String displayName,
    required String timeZoneId,
  }) async {
    final result = await _remoteDataSource.register(
      email: email,
      password: password,
      username: username,
      displayName: displayName,
      timeZoneId: timeZoneId,
    );

    return _persistAndMap(result);
  }

  @override
  Future<UserSummary> login({required String email, required String password}) async {
    final result = await _remoteDataSource.login(email: email, password: password);
    return _persistAndMap(result);
  }

  @override
  Future<void> logout() async {
    final refreshToken = await _tokenStorage.readRefreshToken();
    if (refreshToken != null) {
      try {
        await _remoteDataSource.logout(refreshToken);
      } catch (_) {
        // Best-effort — clear local state regardless of whether the server call succeeds.
      }
    }
    await _tokenStorage.clear();
    await _cachedUserStorage.clear();
  }

  @override
  Future<UserSummary?> restoreSession() async {
    final refreshToken = await _tokenStorage.readRefreshToken();
    if (refreshToken == null) return null;

    return _cachedUserStorage.read();
  }

  Future<UserSummary> _persistAndMap(AuthResultDto result) async {
    await _tokenStorage.saveTokens(accessToken: result.accessToken, refreshToken: result.refreshToken);

    final user = UserSummary(
      id: result.user.id,
      username: result.user.username,
      displayName: result.user.displayName,
      friendCode: result.user.friendCode,
    );

    await _cachedUserStorage.save(user);

    return user;
  }
}
