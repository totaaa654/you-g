import '../../../../core/models/availability_status.dart';
import '../../../../core/models/time_slot.dart';
import '../../domain/entities/availability_instance.dart';
import '../../domain/entities/overlap_window.dart';
import '../../domain/repositories/availability_repository.dart';
import '../datasources/availability_remote_data_source.dart';
import '../dtos/availability_instance_dto.dart';
import '../dtos/overlap_result_dto.dart';

class AvailabilityRepositoryImpl implements AvailabilityRepository {
  AvailabilityRepositoryImpl(this._remoteDataSource);

  final AvailabilityRemoteDataSource _remoteDataSource;

  @override
  Future<List<AvailabilityInstance>> getMyInstances({required DateTime from, required DateTime to}) async {
    final dtos = await _remoteDataSource.getMyInstances(from: from, to: to);
    return dtos.map(_mapInstance).toList();
  }

  @override
  Future<void> upsertInstances(List<AvailabilityInstance> instances) => _remoteDataSource.upsertInstances([
        for (final instance in instances)
          {
            'date': dateOnly(instance.date),
            'startTime': instance.startTime.toJson(),
            'status': instance.status.toJson(),
          },
      ]);

  @override
  Future<List<OverlapWindow>> getGroupOverlap(
    String groupId, {
    required DateTime from,
    required DateTime to,
    bool weekendOnly = false,
  }) async {
    final result = await _remoteDataSource.getGroupOverlap(groupId, from: from, to: to, weekendOnly: weekendOnly);
    return result.windows.map(_mapWindow).toList();
  }

  AvailabilityInstance _mapInstance(AvailabilityInstanceDto dto) => AvailabilityInstance(
        date: dto.date,
        startTime: TimeSlot.fromJson(dto.startTime),
        status: AvailabilityStatus.fromJson(dto.status),
      );

  OverlapWindow _mapWindow(OverlapWindowDto dto) => OverlapWindow(
        date: dto.date,
        startTime: TimeSlot.fromJson(dto.startTime),
        availableUserIds: dto.availableUserIds,
        availableCount: dto.availableCount,
        totalMembers: dto.totalMembers,
        maybeUserIds: dto.maybeUserIds,
      );
}
