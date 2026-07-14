import 'package:flutter/material.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_card.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../../../friends/domain/entities/friend.dart';

/// There is no activity-feed concept anywhere in the backend (not even a stub endpoint) —
/// this renders illustrative placeholder activity for real friends, same honesty bar as the
/// dummy Notifications feature. Never presented as live data.
class FriendsActivityStrip extends StatelessWidget {
  const FriendsActivityStrip({required this.friends, super.key});

  final List<Friend> friends;

  static const _sampleLines = [
    'set their availability for the weekend',
    'joined a new group',
    'is now available this evening',
    'just RSVP\'d to an event',
  ];

  @override
  Widget build(BuildContext context) {
    if (friends.isEmpty) {
      return const SizedBox.shrink();
    }

    final shown = friends.take(4).toList();

    return AppCard(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          for (var i = 0; i < shown.length; i++) ...[
            Row(
              children: [
                ProfileAvatar(displayName: shown[i].profile.displayName, imageUrl: shown[i].profile.profilePictureUrl, size: 32),
                const SizedBox(width: 10),
                Expanded(
                  child: RichText(
                    text: TextSpan(
                      style: DefaultTextStyle.of(context).style.copyWith(fontSize: 13, color: AppColors.navy),
                      children: [
                        TextSpan(text: shown[i].profile.displayName, style: const TextStyle(fontWeight: FontWeight.w700)),
                        TextSpan(text: ' ${_sampleLines[i % _sampleLines.length]}'),
                      ],
                    ),
                  ),
                ),
              ],
            ),
            if (i != shown.length - 1) const SizedBox(height: 12),
          ],
        ],
      ),
    );
  }
}
