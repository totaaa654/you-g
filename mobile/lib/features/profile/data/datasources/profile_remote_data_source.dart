import 'package:dio/dio.dart';

import '../dtos/profile_dto.dart';
import '../dtos/settings_dto.dart';

class ProfileRemoteDataSource {
  ProfileRemoteDataSource(this._dio);

  final Dio _dio;

  Future<ProfileDto> getMyProfile() async {
    final response = await _dio.get('/users/me');
    return ProfileDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<ProfileDto> updateMyProfile({
    required String displayName,
    String? bio,
    required String timeZoneId,
    String? profilePictureUrl,
  }) async {
    final response = await _dio.patch('/users/me', data: {
      'displayName': displayName,
      'bio': bio,
      'timeZoneId': timeZoneId,
      'profilePictureUrl': profilePictureUrl,
    });
    return ProfileDto.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> deleteMyAccount() => _dio.delete('/users/me');

  Future<SettingsDto> updateMySettings({
    required String themePreference,
    required bool isSearchable,
    required bool notifyOnFriendRequest,
    required bool notifyOnGroupInvite,
    required bool notifyOnEventReminder,
    required bool notifyOnScheduleUpdate,
  }) async {
    final response = await _dio.patch('/users/me/settings', data: {
      'themePreference': themePreference,
      'isSearchable': isSearchable,
      'notifyOnFriendRequest': notifyOnFriendRequest,
      'notifyOnGroupInvite': notifyOnGroupInvite,
      'notifyOnEventReminder': notifyOnEventReminder,
      'notifyOnScheduleUpdate': notifyOnScheduleUpdate,
    });
    return SettingsDto.fromJson(response.data as Map<String, dynamic>);
  }
}
