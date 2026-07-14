import 'package:flutter/material.dart';

import '../models/availability_status.dart';
import '../theme/app_theme.dart';

/// Small color-coded pill showing an availability state — used on friend tiles, calendar
/// cells, and Smart Time Finder member lists so the same status always reads the same way.
class AvailabilityBadge extends StatelessWidget {
  const AvailabilityBadge({required this.status, this.dense = false, super.key});

  final AvailabilityStatus status;
  final bool dense;

  (Color, Color) get _colors => switch (status) {
        AvailabilityStatus.available => (AppColors.availableGreen.withValues(alpha: 0.12), AppColors.availableGreen),
        AvailabilityStatus.busy => (AppColors.busyRed.withValues(alpha: 0.12), AppColors.busyRed),
        AvailabilityStatus.maybe => (AppColors.maybeGold.withValues(alpha: 0.22), const Color(0xFF8A6A1A)),
        AvailabilityStatus.unknown => (AppColors.fog, AppColors.unknownGray),
      };

  @override
  Widget build(BuildContext context) {
    final (background, foreground) = _colors;

    if (dense) {
      return Container(
        width: 10,
        height: 10,
        decoration: BoxDecoration(color: foreground, shape: BoxShape.circle),
      );
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(color: background, borderRadius: BorderRadius.circular(999)),
      child: Text(
        status.label,
        style: TextStyle(color: foreground, fontSize: 11.5, fontWeight: FontWeight.w700),
      ),
    );
  }
}
