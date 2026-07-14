import 'package:dio/dio.dart';

import '../dtos/availability_instance_dto.dart';
import '../dtos/overlap_result_dto.dart';

String _dateOnly(DateTime date) => date.toIso8601String().split('T').first;

class AvailabilityRemoteDataSource {
  AvailabilityRemoteDataSource(this._dio);

  final Dio _dio;

  Future<List<AvailabilityInstanceDto>> getMyInstances({required DateTime from, required DateTime to}) async {
    final response = await _dio.get(
      '/availability/me/instances',
      queryParameters: {'from': _dateOnly(from), 'to': _dateOnly(to)},
    );
    return (response.data as List).map((e) => AvailabilityInstanceDto.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<void> upsertInstances(List<Map<String, String>> instances) =>
      _dio.patch('/availability/me/instances', data: instances);

  Future<OverlapResultDto> getGroupOverlap(
    String groupId, {
    required DateTime from,
    required DateTime to,
    required bool weekendOnly,
    String? preferredDayparts,
  }) async {
    final response = await _dio.get('/groups/$groupId/overlap', queryParameters: {
      'from': _dateOnly(from),
      'to': _dateOnly(to),
      'weekendOnly': weekendOnly,
      if (preferredDayparts != null) 'preferredDayparts': preferredDayparts,
    });
    return OverlapResultDto.fromJson(response.data as Map<String, dynamic>);
  }
}

String dateOnly(DateTime date) => _dateOnly(date);
