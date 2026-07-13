import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:youg/core/error/failure.dart';
import 'package:youg/features/auth/domain/entities/user_summary.dart';
import 'package:youg/features/auth/presentation/providers/auth_controller.dart';

import '../fakes/fake_auth_repository.dart';

const _testUser = UserSummary(id: '1', username: 'mayag', displayName: 'Maya', friendCode: 'YG-000000');

Future<void> _waitForRestore(AuthController controller) async {
  // _restoreSession() runs as an unawaited Future in the constructor — give it a tick.
  await Future<void>.delayed(Duration.zero);
}

void main() {
  group('AuthController', () {
    test('restores a cached session on construction', () async {
      final repo = FakeAuthRepository()..restoredUser = _testUser;
      final controller = AuthController(repo);

      await _waitForRestore(controller);

      expect(controller.state.value, _testUser);
    });

    test('starts unauthenticated when there is no cached session', () async {
      final repo = FakeAuthRepository();
      final controller = AuthController(repo);

      await _waitForRestore(controller);

      expect(controller.state.value, isNull);
    });

    test('login success sets state to the returned user', () async {
      final repo = FakeAuthRepository();
      final controller = AuthController(repo);
      await _waitForRestore(controller);

      await controller.login(email: 'maya@example.com', password: 'correct-horse-battery');

      expect(controller.state.value, _testUser);
      expect(controller.state.hasError, isFalse);
    });

    test('login failure sets an AuthenticationFailure error state, not a crash', () async {
      final repo = FakeAuthRepository()..throwOnLogin = true;
      final controller = AuthController(repo);
      await _waitForRestore(controller);

      await controller.login(email: 'maya@example.com', password: 'wrong-password');

      expect(controller.state.hasError, isTrue);
      expect(controller.state.error, isA<AuthenticationFailure>());
    });

    test('register failure sets a ConflictFailure error state', () async {
      final repo = FakeAuthRepository()..throwOnRegister = true;
      final controller = AuthController(repo);
      await _waitForRestore(controller);

      await controller.register(
        email: 'maya@example.com',
        password: 'correct-horse-battery',
        username: 'mayag',
        displayName: 'Maya',
        timeZoneId: 'UTC',
      );

      expect(controller.state.error, isA<ConflictFailure>());
    });

    test('logout clears state and calls the repository', () async {
      final repo = FakeAuthRepository()..restoredUser = _testUser;
      final controller = AuthController(repo);
      await _waitForRestore(controller);

      await controller.logout();

      expect(controller.state.value, isNull);
      expect(repo.logoutCalled, isTrue);
    });
  });
}
