import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'core/push/push_notification_service.dart';
import 'core/router/app_router.dart';
import 'core/theme/app_theme.dart';
import 'features/auth/presentation/providers/auth_controller.dart';
import 'features/notifications/presentation/providers/notifications_providers.dart';
import 'firebase_options.dart';

/// Top-level, not a closure — required by `FirebaseMessaging.onBackgroundMessage`, which runs
/// this in its own isolate on Android/iOS (unsupported on web; the service worker handles that).
@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  await Firebase.initializeApp(options: DefaultFirebaseOptions.currentPlatform);
}

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp(options: DefaultFirebaseOptions.currentPlatform);

  if (!kIsWeb) {
    FirebaseMessaging.onBackgroundMessage(_firebaseMessagingBackgroundHandler);
  }

  runApp(const ProviderScope(child: YouGApp()));
}

class YouGApp extends ConsumerWidget {
  const YouGApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(appRouterProvider);

    ref.listen(authControllerProvider, (previous, next) {
      final wasSignedIn = previous != null && isAuthenticated(previous);
      final isSignedIn = isAuthenticated(next);

      if (isSignedIn && !wasSignedIn) {
        ref.read(pushNotificationServiceProvider).registerForCurrentUser();
      } else if (!isSignedIn && wasSignedIn) {
        ref.read(pushNotificationServiceProvider).unregisterForCurrentUser();
      }
    });

    ref.listen(firebaseOnMessageProvider, (previous, next) {
      // App is already open when the push arrives — sync the in-app list instead of
      // surfacing a redundant system banner on top of the UI the user is looking at.
      if (next.hasValue) {
        ref.read(notificationsListProvider.notifier).refresh();
      }
    });

    return MaterialApp.router(
      title: 'You G?',
      theme: AppTheme.light(),
      darkTheme: AppTheme.dark(),
      routerConfig: router,
    );
  }
}
