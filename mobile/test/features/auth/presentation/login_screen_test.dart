import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:youg/features/auth/presentation/providers/auth_providers.dart';
import 'package:youg/features/auth/presentation/screens/login_screen.dart';

import '../fakes/fake_auth_repository.dart';

Widget _wrap(FakeAuthRepository repo) {
  final router = GoRouter(routes: [
    GoRoute(path: '/', builder: (context, state) => const LoginScreen()),
    GoRoute(path: '/register', builder: (context, state) => const Scaffold(body: Text('Register Screen'))),
  ]);

  return ProviderScope(
    overrides: [authRepositoryProvider.overrideWithValue(repo)],
    child: MaterialApp.router(routerConfig: router),
  );
}

void main() {
  testWidgets('shows validation errors for empty fields', (tester) async {
    await tester.pumpWidget(_wrap(FakeAuthRepository()));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Log in'));
    await tester.pump();

    expect(find.text('Enter a valid email'), findsOneWidget);
    expect(find.text('Enter your password'), findsOneWidget);
  });

  testWidgets('successful login does not show an error snackbar', (tester) async {
    await tester.pumpWidget(_wrap(FakeAuthRepository()));
    await tester.pumpAndSettle();

    await tester.enterText(find.widgetWithText(TextFormField, 'Email'), 'maya@example.com');
    await tester.enterText(find.widgetWithText(TextFormField, 'Password'), 'correct-horse-battery');
    await tester.tap(find.text('Log in'));
    await tester.pumpAndSettle();

    expect(find.byType(SnackBar), findsNothing);
  });

  testWidgets('failed login shows the mapped error message in a snackbar', (tester) async {
    await tester.pumpWidget(_wrap(FakeAuthRepository()..throwOnLogin = true));
    await tester.pumpAndSettle();

    await tester.enterText(find.widgetWithText(TextFormField, 'Email'), 'maya@example.com');
    await tester.enterText(find.widgetWithText(TextFormField, 'Password'), 'wrong-password');
    await tester.tap(find.text('Log in'));
    await tester.pumpAndSettle();

    expect(find.text('Invalid email or password.'), findsOneWidget);
  });

  testWidgets('tapping sign up navigates to the register route', (tester) async {
    await tester.pumpWidget(_wrap(FakeAuthRepository()));
    await tester.pumpAndSettle();

    await tester.tap(find.text("Don't have an account? Sign up"));
    await tester.pumpAndSettle();

    expect(find.text('Register Screen'), findsOneWidget);
  });
}
