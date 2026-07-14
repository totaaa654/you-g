import '../entities/profile.dart';
import '../entities/settings.dart';
import '../entities/theme_preference.dart';

abstract class ProfileRepository {
  Future<Profile> getMyProfile();

  Future<Profile> updateMyProfile({
    required String displayName,
    String? bio,
    required String timeZoneId,
    String? profilePictureUrl,
  });

  Future<void> deleteMyAccount();

  Future<Settings> updateMySettings({
    required ThemePreference themePreference,
    required bool isSearchable,
    required bool notifyOnFriendRequest,
    required bool notifyOnGroupInvite,
    required bool notifyOnEventReminder,
    required bool notifyOnScheduleUpdate,
  });
}
