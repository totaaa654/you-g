import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/error/failure_mapper.dart';
import '../../domain/entities/user_summary.dart';
import '../../domain/repositories/auth_repository.dart';
import 'auth_providers.dart';

/// null user = signed out. AsyncValue.loading() while an operation is in flight,
/// AsyncValue.error(Failure) if the last operation failed.
class AuthController extends StateNotifier<AsyncValue<UserSummary?>> {
  AuthController(this._repository) : super(const AsyncValue.loading()) {
    _restoreSession();
  }

  final AuthRepository _repository;

  Future<void> _restoreSession() async {
    final user = await _repository.restoreSession();
    state = AsyncValue.data(user);
  }

  Future<void> register({
    required String email,
    required String password,
    required String username,
    required String displayName,
    required String timeZoneId,
  }) async {
    state = const AsyncValue.loading();
    try {
      final user = await _repository.register(
        email: email,
        password: password,
        username: username,
        displayName: displayName,
        timeZoneId: timeZoneId,
      );
      state = AsyncValue.data(user);
    } on DioException catch (e, st) {
      state = AsyncValue.error(FailureMapper.fromDioException(e), st);
    }
  }

  Future<void> login({required String email, required String password}) async {
    state = const AsyncValue.loading();
    try {
      final user = await _repository.login(email: email, password: password);
      state = AsyncValue.data(user);
    } on DioException catch (e, st) {
      state = AsyncValue.error(FailureMapper.fromDioException(e), st);
    }
  }

  Future<void> logout() async {
    await _repository.logout();
    state = const AsyncValue.data(null);
  }
}

final authControllerProvider = StateNotifierProvider<AuthController, AsyncValue<UserSummary?>>(
  (ref) => AuthController(ref.watch(authRepositoryProvider)),
);

/// Convenience: does the current state definitively represent a signed-in user?
/// (Not "loading" and not "signed out.")
bool isAuthenticated(AsyncValue<UserSummary?> state) => state.valueOrNull != null;
