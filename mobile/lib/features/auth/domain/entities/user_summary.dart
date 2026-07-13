import 'package:freezed_annotation/freezed_annotation.dart';

part 'user_summary.freezed.dart';

@freezed
abstract class UserSummary with _$UserSummary {
  const factory UserSummary({
    required String id,
    required String username,
    required String displayName,
    required String friendCode,
  }) = _UserSummary;
}
