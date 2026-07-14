import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/network/network_providers.dart';
import '../../data/datasources/events_remote_data_source.dart';
import '../../data/repositories/events_repository_impl.dart';
import '../../domain/entities/event.dart';
import '../../domain/entities/event_detail.dart';
import '../../domain/repositories/events_repository.dart';

final eventsRemoteDataSourceProvider =
    Provider<EventsRemoteDataSource>((ref) => EventsRemoteDataSource(ref.watch(dioProvider)));

final eventsRepositoryProvider =
    Provider<EventsRepository>((ref) => EventsRepositoryImpl(ref.watch(eventsRemoteDataSourceProvider)));

final groupEventsProvider = FutureProvider.family.autoDispose<List<Event>, String>(
  (ref, groupId) => ref.watch(eventsRepositoryProvider).getGroupEvents(groupId),
);

final eventDetailProvider = AsyncNotifierProvider.family<EventDetailNotifier, EventDetail, String>(
  EventDetailNotifier.new,
);

class EventDetailNotifier extends FamilyAsyncNotifier<EventDetail, String> {
  @override
  Future<EventDetail> build(String arg) => ref.watch(eventsRepositoryProvider).getEventById(arg);

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => ref.read(eventsRepositoryProvider).getEventById(arg));
  }
}
