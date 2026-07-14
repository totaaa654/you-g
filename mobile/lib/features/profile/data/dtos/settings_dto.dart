import 'package:freezed_annotation/freezed_annotation.dart';

part 'settings_dto.freezed.dart';
part 'settings_dto.g.dart';

/// Mirrors backend `SettingsDto` exactly. `themePreference` stays a raw wire string.
@freezed
abstract class SettingsDto with _$SettingsDto {
  const factory SettingsDto({
    required String themePreference,
    required bool isSearchable,
    required bool notifyOnFriendRequest,
    required bool notifyOnGroupInvite,
    required bool notifyOnEventReminder,
    required bool notifyOnScheduleUpdate,
  }) = _SettingsDto;

  factory SettingsDto.fromJson(Map<String, dynamic> json) => _$SettingsDtoFromJson(json);
}
