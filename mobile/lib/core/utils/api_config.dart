import 'dart:io' show Platform;

import 'package:flutter/foundation.dart' show kIsWeb;

/// Resolves the API base URL for local development. Overridable at build time with
/// `--dart-define=API_BASE_URL=https://your-staging-url` for staging/prod builds.
class ApiConfig {
  static const String _override = String.fromEnvironment('API_BASE_URL');

  static String get baseUrl {
    if (_override.isNotEmpty) return _override;

    // Android emulator can't reach the host machine via "localhost" — 10.0.2.2 is the
    // documented alias for the host loopback interface. iOS simulator and web can use
    // localhost directly since they share the host's network namespace.
    if (!kIsWeb && Platform.isAndroid) {
      return 'http://10.0.2.2:5283';
    }

    return 'http://localhost:5283';
  }

  static const String apiVersion = 'v1';
}
