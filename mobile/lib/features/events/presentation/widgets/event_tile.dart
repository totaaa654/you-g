import 'package:flutter/material.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../domain/entities/event.dart';
import '../../domain/entities/event_status.dart';

class EventTile extends StatelessWidget {
  const EventTile({required this.event, this.onTap, super.key});

  final Event event;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final (label, color) = switch (event.status) {
      EventStatus.proposed => ('Voting open', AppColors.maybeGold),
      EventStatus.confirmed => ('Confirmed', AppColors.availableGreen),
      EventStatus.cancelled => ('Cancelled', AppColors.busyRed),
    };

    return AppCard(
      onTap: onTap,
      padding: const EdgeInsets.all(14),
      child: Row(
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(14)),
            child: const Icon(Icons.event_rounded, color: AppColors.accentBlue),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(event.title, style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 2),
                Text(label, style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: color)),
              ],
            ),
          ),
          const Icon(Icons.chevron_right_rounded, color: AppColors.unknownGray),
        ],
      ),
    );
  }
}
