import 'package:dio/dio.dart';

import '../storage/token_storage.dart';
import '../utils/api_config.dart';

/// Centralizes token attach + refresh-on-401 in one place instead of scattering it across
/// every API call site (docs/02-ARCHITECTURE.md Section 6.2).
class DioClient {
  DioClient(this._tokenStorage) {
    _dio = Dio(BaseOptions(
      baseUrl: '${ApiConfig.baseUrl}/api/${ApiConfig.apiVersion}',
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
    ));

    // A separate, interceptor-free instance for the refresh call itself — otherwise a
    // failed refresh would trigger its own 401 handling and loop forever.
    _refreshDio = Dio(BaseOptions(baseUrl: _dio.options.baseUrl));

    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await _tokenStorage.readAccessToken();
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        handler.next(options);
      },
      onError: (error, handler) async {
        final alreadyRetried = error.requestOptions.extra['retried'] == true;

        if (error.response?.statusCode == 401 && !alreadyRetried) {
          final refreshed = await _tryRefresh();

          if (refreshed) {
            final retryOptions = error.requestOptions..extra['retried'] = true;
            final newToken = await _tokenStorage.readAccessToken();
            retryOptions.headers['Authorization'] = 'Bearer $newToken';

            try {
              final response = await _dio.fetch(retryOptions);
              return handler.resolve(response);
            } catch (_) {
              // Fall through — surface the original 401 below.
            }
          } else {
            await _tokenStorage.clear();
          }
        }

        handler.next(error);
      },
    ));
  }

  late final Dio _dio;
  late final Dio _refreshDio;
  final TokenStorage _tokenStorage;

  Dio get dio => _dio;

  Future<bool> _tryRefresh() async {
    final refreshToken = await _tokenStorage.readRefreshToken();
    if (refreshToken == null) return false;

    try {
      final response = await _refreshDio.post('/auth/refresh', data: {'refreshToken': refreshToken});
      final newAccessToken = response.data['accessToken'] as String;
      final newRefreshToken = response.data['refreshToken'] as String;
      await _tokenStorage.saveTokens(accessToken: newAccessToken, refreshToken: newRefreshToken);
      return true;
    } catch (_) {
      return false;
    }
  }
}
