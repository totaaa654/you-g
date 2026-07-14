import 'package:flutter/material.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../domain/entities/event_location_option.dart';

class EventLocationOptionTile extends StatelessWidget {
  const EventLocationOptionTile({required this.option, required this.onToggleVote, super.key});

  final EventLocationOption option;
  final VoidCallback onToggleVote;

  @override
  Widget build(BuildContext context) {
    return AppCard(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(option.name, style: Theme.of(context).textTheme.titleMedium),
                if (option.address != null && option.address!.isNotEmpty)
                  Text(option.address!, style: Theme.of(context).textTheme.bodyMedium),
              ],
            ),
          ),
          Text('${option.voteCount}', style: const TextStyle(fontWeight: FontWeight.w700, color: AppColors.navy)),
          const SizedBox(width: 6),
          IconButton(
            onPressed: onToggleVote,
            icon: Icon(
              option.hasCurrentUserVoted ? Icons.check_circle_rounded : Icons.check_circle_outline_rounded,
              color: option.hasCurrentUserVoted ? AppColors.availableGreen : AppColors.unknownGray,
            ),
          ),
        ],
      ),
    );
  }
}
