import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/providers/auth_controller.dart';
import '../../features/auth/presentation/screens/forgot_password_screen.dart';
import '../../features/auth/presentation/screens/login_screen.dart';
import '../../features/auth/presentation/screens/register_screen.dart';
import '../../features/auth/presentation/screens/reset_password_screen.dart';
import '../../features/availability/presentation/screens/calendar_screen.dart';
import '../../features/availability/presentation/screens/smart_time_finder_screen.dart';
import '../../features/events/presentation/screens/create_event_screen.dart';
import '../../features/events/presentation/screens/event_detail_screen.dart';
import '../../features/friends/presentation/screens/friends_screen.dart';
import '../../features/groups/presentation/screens/group_detail_screen.dart';
import '../../features/groups/presentation/screens/groups_screen.dart';
import '../../features/home/presentation/screens/home_screen.dart';
import '../../features/notifications/presentation/screens/notifications_screen.dart';
import '../../features/profile/presentation/screens/profile_screen.dart';
import '../../features/settings/presentation/screens/settings_screen.dart';
import '../../features/start/presentation/screens/start_screen.dart';
import '../widgets/bottom_nav_shell.dart';

const _preLoginRoutes = {'/start', '/login', '/register', '/forgot-password', '/reset-password'};

final appRouterProvider = Provider<GoRouter>((ref) {
  // Riverpod's Listenable bridge — GoRouter re-evaluates `redirect` whenever this notifies,
  // which happens on every authControllerProvider state change (login/logout/restore).
  final refreshListenable = GoRouterRefreshStream(ref);

  return GoRouter(
    initialLocation: '/start',
    refreshListenable: refreshListenable,
    redirect: (context, state) {
      final authState = ref.read(authControllerProvider);

      // Still restoring the session on app startup — don't redirect yet.
      if (authState.isLoading) return null;

      final loggedIn = isAuthenticated(authState);
      final onPreLoginRoute = _preLoginRoutes.contains(state.matchedLocation);

      if (!loggedIn && !onPreLoginRoute) return '/start';
      if (loggedIn && onPreLoginRoute) return '/home';

      return null;
    },
    routes: [
      GoRoute(path: '/start', builder: (context, state) => const StartScreen()),
      GoRoute(path: '/login', builder: (context, state) => const LoginScreen()),
      GoRoute(path: '/register', builder: (context, state) => const RegisterScreen()),
      GoRoute(path: '/forgot-password', builder: (context, state) => const ForgotPasswordScreen()),
      GoRoute(
        path: '/reset-password',
        builder: (context, state) => ResetPasswordScreen(
          email: state.uri.queryParameters['email'],
          code: state.uri.queryParameters['code'],
        ),
      ),
      StatefulShellRoute.indexedStack(
        builder: (context, state, navigationShell) => BottomNavShell(navigationShell: navigationShell),
        branches: [
          StatefulShellBranch(routes: [GoRoute(path: '/home', builder: (context, state) => const HomeScreen())]),
          StatefulShellBranch(routes: [GoRoute(path: '/friends', builder: (context, state) => const FriendsScreen())]),
          StatefulShellBranch(routes: [GoRoute(path: '/groups', builder: (context, state) => const GroupsScreen())]),
          StatefulShellBranch(routes: [GoRoute(path: '/calendar', builder: (context, state) => const CalendarScreen())]),
          StatefulShellBranch(routes: [GoRoute(path: '/profile', builder: (context, state) => const ProfileScreen())]),
        ],
      ),
      GoRoute(
        path: '/groups/:id',
        builder: (context, state) => GroupDetailScreen(groupId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: '/groups/:id/smart-time-finder',
        builder: (context, state) => SmartTimeFinderScreen(groupId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: '/groups/:id/events/create',
        builder: (context, state) {
          final dateParam = state.uri.queryParameters['date'];
          return CreateEventScreen(
            groupId: state.pathParameters['id']!,
            initialDate: dateParam != null ? DateTime.tryParse(dateParam) : null,
            initialStartTime: state.uri.queryParameters['startTime'],
          );
        },
      ),
      GoRoute(
        path: '/events/:id',
        builder: (context, state) => EventDetailScreen(eventId: state.pathParameters['id']!),
      ),
      GoRoute(path: '/notifications', builder: (context, state) => const NotificationsScreen()),
      GoRoute(path: '/settings', builder: (context, state) => const SettingsScreen()),
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
