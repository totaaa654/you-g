import '../../../../core/models/availability_status.dart';
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
            'daypart': instance.daypart.toJson(),
            'status': instance.status.toJson(),
          },
      ]);

  @override
  Future<List<OverlapWindow>> getGroupOverlap(
    String groupId, {
    required DateTime from,
    required DateTime to,
    bool weekendOnly = false,
    List<Daypart>? preferredDayparts,
  }) async {
    final result = await _remoteDataSource.getGroupOverlap(
      groupId,
      from: from,
      to: to,
      weekendOnly: weekendOnly,
      preferredDayparts: preferredDayparts?.map((d) => d.toJson()).join(','),
    );
    return result.windows.map(_mapWindow).toList();
  }

  AvailabilityInstance _mapInstance(AvailabilityInstanceDto dto) => AvailabilityInstance(
        date: dto.date,
        daypart: Daypart.fromJson(dto.daypart),
        status: AvailabilityStatus.fromJson(dto.status),
      );

  OverlapWindow _mapWindow(OverlapWindowDto dto) => OverlapWindow(
        date: dto.date,
        daypart: Daypart.fromJson(dto.daypart),
        availableUserIds: dto.availableUserIds,
        availableCount: dto.availableCount,
        totalMembers: dto.totalMembers,
        maybeUserIds: dto.maybeUserIds,
      );
}
