import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/mascot.dart';
import '../../../availability/domain/entities/overlap_window.dart';
import '../../../groups/domain/entities/group.dart';

class SuggestedMeetupCard extends StatelessWidget {
  const SuggestedMeetupCard({required this.group, required this.window, required this.onTap, super.key});

  final Group group;
  final OverlapWindow window;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: AppColors.navy,
      borderRadius: BorderRadius.circular(24),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Row(
            children: [
              const Mascot(size: 64),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Suggested meetup',
                      style: TextStyle(color: AppColors.gold, fontWeight: FontWeight.w700, fontSize: 12, letterSpacing: 0.4),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      '${group.name} · ${DateFormat('EEE, MMM d').format(window.date)} ${window.daypart.label}',
                      style: const TextStyle(color: Colors.white, fontWeight: FontWeight.w700, fontSize: 15),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '${window.availableCount} of ${window.totalMembers} are free',
                      style: TextStyle(color: Colors.white.withValues(alpha: 0.75), fontSize: 12.5),
                    ),
                  ],
                ),
              ),
              const Icon(Icons.arrow_forward_ios_rounded, color: Colors.white, size: 16),
            ],
          ),
        ),
      ),
    );
  }
}
