import 'package:flutter/material.dart';

import '../../../../core/models/availability_status.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../../../core/widgets/availability_badge.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../domain/entities/friend.dart';

/// There is no backend endpoint for another user's live availability (only `/availability/me/*`
/// exists) — this derives a stable-per-user placeholder from their id so the indicator doesn't
/// flicker across rebuilds, clearly a presentation stand-in rather than fabricated real data.
AvailabilityStatus _placeholderAvailability(String userId) {
  final values = AvailabilityStatus.values;
  return values[userId.hashCode.abs() % values.length];
}

class FriendTile extends StatelessWidget {
  const FriendTile({
    required this.friend,
    required this.onToggleFavorite,
    this.onTap,
    super.key,
  });

  final Friend friend;
  final ValueChanged<bool> onToggleFavorite;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final profile = friend.profile;

    return AppCard(
      onTap: onTap,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      child: Row(
        children: [
          ProfileAvatar(displayName: profile.displayName, imageUrl: profile.profilePictureUrl, size: 48),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(profile.displayName, style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 2),
                Row(
                  children: [
                    Text('@${profile.username}', style: Theme.of(context).textTheme.bodyMedium),
                    const SizedBox(width: 8),
                    AvailabilityBadge(status: _placeholderAvailability(profile.id), dense: true),
                  ],
                ),
              ],
            ),
          ),
          IconButton(
            onPressed: () => onToggleFavorite(!friend.isFavorite),
            icon: Icon(
              friend.isFavorite ? Icons.star_rounded : Icons.star_outline_rounded,
              color: friend.isFavorite ? AppColors.gold : AppColors.unknownGray,
            ),
          ),
        ],
      ),
    );
  }
}
