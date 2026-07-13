import 'dart:convert';

import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../../domain/entities/user_summary.dart';

/// Caches the logged-in user's summary locally so the app can restore session state on
/// launch without a `/users/me` endpoint (not built yet — see Profile feature).
class CachedUserStorage {
  CachedUserStorage(this._storage);

  final FlutterSecureStorage _storage;

  static const _key = 'cached_user';

  Future<void> save(UserSummary user) => _storage.write(
        key: _key,
        value: jsonEncode({
          'id': user.id,
          'username': user.username,
          'displayName': user.displayName,
          'friendCode': user.friendCode,
        }),
      );

  Future<UserSummary?> read() async {
    final raw = await _storage.read(key: _key);
    if (raw == null) return null;

    final json = jsonDecode(raw) as Map<String, dynamic>;
    return UserSummary(
      id: json['id'] as String,
      username: json['username'] as String,
      displayName: json['displayName'] as String,
      friendCode: json['friendCode'] as String,
    );
  }

  Future<void> clear() => _storage.delete(key: _key);
}
