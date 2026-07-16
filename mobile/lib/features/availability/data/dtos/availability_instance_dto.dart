import 'package:freezed_annotation/freezed_annotation.dart';

part 'availability_instance_dto.freezed.dart';
part 'availability_instance_dto.g.dart';

/// Mirrors backend `AvailabilityInstanceDto`. `date` is a `DateOnly` ("yyyy-MM-dd") on the
/// wire — `DateTime.parse` handles that format fine, so no custom converter is needed.
/// `startTime` is a `TimeOnly` ("HH:mm:ss") — each row covers startTime to startTime+30min.
/// `startTime`/`status` stay raw wire strings, mapped to value types by the repository.
@freezed
abstract class AvailabilityInstanceDto with _$AvailabilityInstanceDto {
  const factory AvailabilityInstanceDto({
    required DateTime date,
    required String startTime,
    required String status,
  }) = _AvailabilityInstanceDto;

  factory AvailabilityInstanceDto.fromJson(Map<String, dynamic> json) => _$AvailabilityInstanceDtoFromJson(json);
}
