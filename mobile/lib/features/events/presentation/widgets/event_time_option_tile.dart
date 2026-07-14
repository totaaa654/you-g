import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../domain/entities/event_time_option.dart';

class EventTimeOptionTile extends StatelessWidget {
  const EventTimeOptionTile({required this.option, required this.onToggleVote, super.key});

  final EventTimeOption option;
  final VoidCallback onToggleVote;

  @override
  Widget build(BuildContext context) {
    return AppCard(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      child: Row(
        children: [
          Expanded(
            child: Text(
              DateFormat('EEE, MMM d · h:mm a').format(option.startUtc.toLocal()),
              style: Theme.of(context).textTheme.titleMedium,
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
