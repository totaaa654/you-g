import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../data/repositories/fake_notifications_repository.dart';
import '../../domain/entities/app_notification.dart';
import '../../domain/repositories/notifications_repository.dart';

final notificationsRepositoryProvider =
    Provider<NotificationsRepository>((ref) => FakeNotificationsRepository());

final notificationsListProvider =
    AsyncNotifierProvider<NotificationsListNotifier, List<AppNotification>>(NotificationsListNotifier.new);

class NotificationsListNotifier extends AsyncNotifier<List<AppNotification>> {
  @override
  Future<List<AppNotification>> build() => ref.watch(notificationsRepositoryProvider).getNotifications();

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
}
