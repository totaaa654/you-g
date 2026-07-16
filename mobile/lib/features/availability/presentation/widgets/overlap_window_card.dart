import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../domain/entities/merged_overlap_window.dart';

class OverlapWindowCard extends StatelessWidget {
  const OverlapWindowCard({
    required this.window,
    this.highlighted = false,
    this.onTap,
    super.key,
  });

  final MergedOverlapWindow window;
  final bool highlighted;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final ratio = window.totalMembers == 0 ? 0.0 : window.availableCount / window.totalMembers;

    return AppCard(
      onTap: onTap,
      color: highlighted ? AppColors.navy : Colors.white,
      padding: const EdgeInsets.all(18),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(Icons.event_available_rounded, color: highlighted ? AppColors.gold : AppColors.accentBlue),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  '${DateFormat('EEEE, MMM d').format(window.date)} · ${window.startTime.label} - ${window.endTime.label}',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(color: highlighted ? Colors.white : null),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          ClipRRect(
            borderRadius: BorderRadius.circular(999),
            child: LinearProgressIndicator(
              value: ratio,
              minHeight: 8,
              backgroundColor: highlighted ? Colors.white.withValues(alpha: 0.2) : AppColors.fog,
              valueColor: AlwaysStoppedAnimation(highlighted ? AppColors.gold : AppColors.availableGreen),
            ),
          ),
          const SizedBox(height: 8),
          Text(
            '${window.availableCount} of ${window.totalMembers} available',
            style: TextStyle(
              color: highlighted ? Colors.white.withValues(alpha: 0.85) : AppColors.navy.withValues(alpha: 0.7),
              fontWeight: FontWeight.w600,
              fontSize: 13,
            ),
          ),
        ],
      ),
    );
  }
}
