import '../entities/availability_instance.dart';
import '../entities/overlap_window.dart';

abstract class AvailabilityRepository {
  Future<List<AvailabilityInstance>> getMyInstances({required DateTime from, required DateTime to});

  /// Upserts one-off overrides — matches `PATCH /availability/me/instances`'s batch contract.
  Future<void> upsertInstances(List<AvailabilityInstance> instances);

  Future<List<OverlapWindow>> getGroupOverlap(
    String groupId, {
    required DateTime from,
    required DateTime to,
    bool weekendOnly = false,
  });
}
