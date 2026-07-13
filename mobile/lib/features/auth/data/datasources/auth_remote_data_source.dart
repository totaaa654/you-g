import 'package:dio/dio.dart';

import '../dtos/auth_result_dto.dart';

class AuthRemoteDataSource {
  AuthRemoteDataSource(this._dio);

  final Dio _dio;

  Future<AuthResultDto> register({
    required String email,
    required String password,
    required String username,
    required String displayName,
    required String timeZoneId,
  }) async {
    final response = await _dio.post('/auth/register', data: {
      'email': email,
      'password': password,
      'username': username,
      'displayName': displayName,
      'timeZoneId': timeZoneId,
    });

    return AuthResultDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AuthResultDto> login({required String email, required String password}) async {
    final response = await _dio.post('/auth/login', data: {'email': email, 'password': password});
    return AuthResultDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> logout(String refreshToken) async {
    await _dio.post('/auth/logout', data: {'refreshToken': refreshToken});
  }
}
