import 'package:freezed_annotation/freezed_annotation.dart';

part 'availability_rule_dto.freezed.dart';
part 'availability_rule_dto.g.dart';

/// Mirrors backend `AvailabilityRuleDto`. `dayOfWeek` is .NET's built-in `DayOfWeek` enum,
/// serialized as its English name ("Sunday".."Saturday") by the same global string converter.
@freezed
abstract class AvailabilityRuleDto with _$AvailabilityRuleDto {
  const factory AvailabilityRuleDto({
    required String id,
    required String dayOfWeek,
    required String daypart,
    required String status,
    required DateTime effectiveFrom,
    DateTime? effectiveUntil,
  }) = _AvailabilityRuleDto;

  factory AvailabilityRuleDto.fromJson(Map<String, dynamic> json) => _$AvailabilityRuleDtoFromJson(json);
}
