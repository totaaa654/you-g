import 'dart:io' show Platform;

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../features/notifications/presentation/providers/notifications_providers.dart';

/// Public key from Firebase Console > Cloud Messaging > Web Push certificates.
/// Not a secret — it identifies the sender, same trust level as the values in firebase_options.dart.
const _webVapidKey = 'BG8wY6OVOd59BLpxL-v9gfYDEUfryEQ4L9OyllbC-PioA70CQhw-k1qWxm3-WSdAJP0oNtU0QoFFop8fY3HUsMw';

/// Registers this device's FCM token with the backend on login and drops it on logout.
/// One instance lives for the app's lifetime — see `ref.listen(authControllerProvider, ...)` in main.dart.
class PushNotificationService {
  PushNotificationService(this._ref);

  final Ref _ref;
  String? _registeredToken;

  Future<void> registerForCurrentUser() async {
    final messaging = FirebaseMessaging.instance;
    final settings = await messaging.requestPermission();

    if (settings.authorizationStatus == AuthorizationStatus.denied) {
      return;
    }

    final token = kIsWeb ? await messaging.getToken(vapidKey: _webVapidKey) : await messaging.getToken();
    if (token == null) {
      return;
    }

    await _register(token);
    messaging.onTokenRefresh.listen(_register);
  }

  Future<void> unregisterForCurrentUser() async {
    final token = _registeredToken;
    _registeredToken = null;

    if (token == null) {
      return;
    }

    try {
      await _ref.read(notificationsRepositoryProvider).unregisterDeviceToken(token);
    } catch (_) {
      // Best-effort — a stale token on the backend is pruned automatically the next time a
      // push to it comes back "unregistered", so a failed cleanup here isn't user-visible.
    }
  }

  Future<void> _register(String token) async {
    _registeredToken = token;
    try {
      await _ref.read(notificationsRepositoryProvider).registerDeviceToken(token, _platform);
    } catch (_) {
      // Best-effort — retried on the next app start / token refresh.
    }
  }

  String get _platform {
    if (kIsWeb) return 'Web';
    return Platform.isIOS ? 'Ios' : 'Android';
  }
}

final pushNotificationServiceProvider = Provider<PushNotificationService>(PushNotificationService.new);

/// A push that arrives while the app is already open — used to sync the in-app notifications
/// list instead of relying on the OS-level banner (which foreground messages don't show anyway).
final firebaseOnMessageProvider = StreamProvider<RemoteMessage>((ref) => FirebaseMessaging.onMessage);
