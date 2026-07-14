import 'package:freezed_annotation/freezed_annotation.dart';

import 'theme_preference.dart';

part 'settings.freezed.dart';

@freezed
abstract class Settings with _$Settings {
  const factory Settings({
    required ThemePreference themePreference,
    required bool isSearchable,
    required bool notifyOnFriendRequest,
    required bool notifyOnGroupInvite,
    required bool notifyOnEventReminder,
    required bool notifyOnScheduleUpdate,
  }) = _Settings;
}
