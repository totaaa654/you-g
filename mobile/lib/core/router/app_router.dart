import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/providers/auth_controller.dart';
import '../../features/auth/presentation/screens/login_screen.dart';
import '../../features/auth/presentation/screens/register_screen.dart';
import '../../features/home/presentation/screens/home_screen.dart';

final appRouterProvider = Provider<GoRouter>((ref) {
  // Riverpod's Listenable bridge — GoRouter re-evaluates `redirect` whenever this notifies,
  // which happens on every authControllerProvider state change (login/logout/restore).
  final refreshListenable = GoRouterRefreshStream(ref);

  return GoRouter(
    initialLocation: '/login',
    refreshListenable: refreshListenable,
    redirect: (context, state) {
      final authState = ref.read(authControllerProvider);

      // Still restoring the session on app startup — don't redirect yet.
      if (authState.isLoading) return null;

      final loggedIn = isAuthenticated(authState);
      final onAuthRoute = state.matchedLocation == '/login' || state.matchedLocation == '/register';

      if (!loggedIn && !onAuthRoute) return '/login';
      if (loggedIn && onAuthRoute) return '/';

      return null;
    },
    routes: [
      GoRoute(path: '/login', builder: (context, state) => const LoginScreen()),
      GoRoute(path: '/register', builder: (context, state) => const RegisterScreen()),
      GoRoute(path: '/', builder: (context, state) => const HomeScreen()),
    ],
  );
});

/// Bridges a Riverpod provider's changes into the ChangeNotifier interface GoRouter's
/// `refreshListenable` expects.
class GoRouterRefreshStream extends ChangeNotifier {
  GoRouterRefreshStream(Ref ref) {
    ref.listen(authControllerProvider, (_, __) => notifyListeners());
  }
}
