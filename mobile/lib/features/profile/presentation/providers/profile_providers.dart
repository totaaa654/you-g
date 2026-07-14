import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../data/datasources/profile_remote_data_source.dart';
import '../../data/repositories/profile_repository_impl.dart';
import '../../domain/entities/profile.dart';
import '../../domain/entities/theme_preference.dart';
import '../../domain/repositories/profile_repository.dart';

final profileRemoteDataSourceProvider =
    Provider<ProfileRemoteDataSource>((ref) => ProfileRemoteDataSource(ref.watch(dioProvider)));

final profileRepositoryProvider =
    Provider<ProfileRepository>((ref) => ProfileRepositoryImpl(ref.watch(profileRemoteDataSourceProvider)));

final myProfileProvider = AsyncNotifierProvider<MyProfileNotifier, Profile>(MyProfileNotifier.new);

class MyProfileNotifier extends AsyncNotifier<Profile> {
  @override
  Future<Profile> build() {
    // Re-runs on login/logout/account switch — see the identical comment in
    // `MyGroupsNotifier.build()` for why this is necessary.
    ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
    return ref.watch(profileRepositoryProvider).getMyProfile();
  }

  Future<void> updateProfile({required String displayName, String? bio, required String timeZoneId}) async {
    final updated = await ref.read(profileRepositoryProvider).updateMyProfile(
          displayName: displayName,
          bio: bio,
          timeZoneId: timeZoneId,
          profilePictureUrl: state.valueOrNull?.profilePictureUrl,
        );
    state = AsyncValue.data(updated);
  }

  Future<void> updateSettings({
    required ThemePreference themePreference,
    required bool isSearchable,
    required bool notifyOnFriendRequest,
    required bool notifyOnGroupInvite,
    required bool notifyOnEventReminder,
    required bool notifyOnScheduleUpdate,
  }) async {
    final settings = await ref.read(profileRepositoryProvider).updateMySettings(
          themePreference: themePreference,
          isSearchable: isSearchable,
          notifyOnFriendRequest: notifyOnFriendRequest,
          notifyOnGroupInvite: notifyOnGroupInvite,
          notifyOnEventReminder: notifyOnEventReminder,
          notifyOnScheduleUpdate: notifyOnScheduleUpdate,
        );
    state = state.whenData((profile) => profile.copyWith(settings: settings));
  }
}
