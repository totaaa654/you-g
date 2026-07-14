import '../../domain/entities/profile.dart';
import '../../domain/entities/settings.dart';
import '../../domain/entities/theme_preference.dart';
import '../../domain/repositories/profile_repository.dart';
import '../datasources/profile_remote_data_source.dart';
import '../dtos/profile_dto.dart';
import '../dtos/settings_dto.dart';

class ProfileRepositoryImpl implements ProfileRepository {
  ProfileRepositoryImpl(this._remoteDataSource);

  final ProfileRemoteDataSource _remoteDataSource;

  @override
  Future<Profile> getMyProfile() async => _mapProfile(await _remoteDataSource.getMyProfile());

  @override
  Future<Profile> updateMyProfile({
    required String displayName,
    String? bio,
    required String timeZoneId,
    String? profilePictureUrl,
  }) async =>
      _mapProfile(await _remoteDataSource.updateMyProfile(
        displayName: displayName,
        bio: bio,
        timeZoneId: timeZoneId,
        profilePictureUrl: profilePictureUrl,
      ));

  @override
  Future<void> deleteMyAccount() => _remoteDataSource.deleteMyAccount();

  @override
  Future<Settings> updateMySettings({
    required ThemePreference themePreference,
    required bool isSearchable,
    required bool notifyOnFriendRequest,
    required bool notifyOnGroupInvite,
    required bool notifyOnEventReminder,
    required bool notifyOnScheduleUpdate,
  }) async {
    final dto = await _remoteDataSource.updateMySettings(
      themePreference: themePreference.toJson(),
      isSearchable: isSearchable,
      notifyOnFriendRequest: notifyOnFriendRequest,
      notifyOnGroupInvite: notifyOnGroupInvite,
      notifyOnEventReminder: notifyOnEventReminder,
      notifyOnScheduleUpdate: notifyOnScheduleUpdate,
    );
    return _mapSettings(dto);
  }

  Profile _mapProfile(ProfileDto dto) => Profile(
        id: dto.id,
        email: dto.email,
        username: dto.username,
        displayName: dto.displayName,
        bio: dto.bio,
        profilePictureUrl: dto.profilePictureUrl,
        timeZoneId: dto.timeZoneId,
        friendCode: dto.friendCode,
        createdAt: dto.createdAt,
        settings: _mapSettings(dto.settings),
      );

  Settings _mapSettings(SettingsDto dto) => Settings(
        themePreference: ThemePreference.fromJson(dto.themePreference),
        isSearchable: dto.isSearchable,
        notifyOnFriendRequest: dto.notifyOnFriendRequest,
        notifyOnGroupInvite: dto.notifyOnGroupInvite,
        notifyOnEventReminder: dto.notifyOnEventReminder,
        notifyOnScheduleUpdate: dto.notifyOnScheduleUpdate,
      );
}
