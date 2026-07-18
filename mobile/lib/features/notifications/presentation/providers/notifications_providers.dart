import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../data/datasources/notifications_remote_data_source.dart';
import '../../data/repositories/notifications_repository_impl.dart';
import '../../domain/entities/app_notification.dart';
import '../../domain/repositories/notifications_repository.dart';

final notificationsRemoteDataSourceProvider =
    Provider<NotificationsRemoteDataSource>((ref) => NotificationsRemoteDataSource(ref.watch(dioProvider)));

final notificationsRepositoryProvider = Provider<NotificationsRepository>(
  (ref) => NotificationsRepositoryImpl(ref.watch(notificationsRemoteDataSourceProvider)),
);

final notificationsListProvider =
    AsyncNotifierProvider<NotificationsListNotifier, List<AppNotification>>(NotificationsListNotifier.new);

class NotificationsListNotifier extends AsyncNotifier<List<AppNotification>> {
  @override
  Future<List<AppNotification>> build() {
    // Re-runs on login/logout/account switch — see the identical comment in
    // `MyGroupsNotifier.build()` for why this is necessary.
    ref.watch(authControllerProvider.select((s) => s.valueOrNull?.id));
    return ref.watch(notificationsRepositoryProvider).getNotifications();
  }

  Future<void> markAsRead(String id) async {
    await ref.read(notificationsRepositoryProvider).markAsRead(id);
    state = state.whenData(
      (notifications) => [
        for (final n in notifications)
          if (n.id == id) n.copyWith(isRead: true) else n,
      ],
    );
  }

  Future<void> markAllAsRead() async {
    await ref.read(notificationsRepositoryProvider).markAllAsRead();
    state = state.whenData((notifications) => [for (final n in notifications) n.copyWith(isRead: true)]);
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => ref.read(notificationsRepositoryProvider).getNotifications());
  }
}
