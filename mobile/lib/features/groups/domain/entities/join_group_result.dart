import 'package:freezed_annotation/freezed_annotation.dart';

import 'group.dart';

part 'join_group_result.freezed.dart';

@freezed
abstract class JoinGroupResult with _$JoinGroupResult {
  const factory JoinGroupResult({required bool joined, Group? group}) = _JoinGroupResult;
}
