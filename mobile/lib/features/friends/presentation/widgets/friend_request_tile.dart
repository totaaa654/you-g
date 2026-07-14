import 'package:flutter/material.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../domain/entities/friend_request.dart';

class FriendRequestTile extends StatelessWidget {
  const FriendRequestTile({
    required this.request,
    required this.onAccept,
    required this.onDecline,
    super.key,
  });

  final FriendRequest request;
  final VoidCallback onAccept;
  final VoidCallback onDecline;

  @override
  Widget build(BuildContext context) {
    final profile = request.profile;

    return AppCard(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      child: Row(
        children: [
          ProfileAvatar(displayName: profile.displayName, imageUrl: profile.profilePictureUrl, size: 44),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(profile.displayName, style: Theme.of(context).textTheme.titleMedium),
                Text('@${profile.username}', style: Theme.of(context).textTheme.bodyMedium),
              ],
            ),
          ),
          IconButton(
            onPressed: onDecline,
            icon: const Icon(Icons.close_rounded),
            color: AppColors.unknownGray,
            style: IconButton.styleFrom(backgroundColor: AppColors.fog),
          ),
          const SizedBox(width: 8),
          IconButton(
            onPressed: onAccept,
            icon: const Icon(Icons.check_rounded),
            color: Colors.white,
            style: IconButton.styleFrom(backgroundColor: AppColors.availableGreen),
          ),
        ],
      ),
    );
  }
}
