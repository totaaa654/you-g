import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/models/availability_status.dart';

part 'overlap_window.freezed.dart';

@freezed
abstract class OverlapWindow with _$OverlapWindow {
  const factory OverlapWindow({
    required DateTime date,
    required Daypart daypart,
    required List<String> availableUserIds,
    required int availableCount,
    required int totalMembers,
    required List<String> maybeUserIds,
  }) = _OverlapWindow;
}
