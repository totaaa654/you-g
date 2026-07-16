import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/models/time_slot.dart';

part 'availability_instance.freezed.dart';

@freezed
abstract class AvailabilityInstance with _$AvailabilityInstance {
  const factory AvailabilityInstance({
    required DateTime date,
    required TimeSlot startTime,
    required AvailabilityStatus status,
  }) = _AvailabilityInstance;
}
