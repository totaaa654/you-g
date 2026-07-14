import 'package:flutter/material.dart';

import '../theme/app_theme.dart';

/// Circular avatar — renders the profile picture when one exists, otherwise falls back to
/// the display name's initials over a soft navy tint. Every avatar in the app goes through
/// this widget so the fallback treatment stays consistent.
class ProfileAvatar extends StatelessWidget {
  const ProfileAvatar({
    required this.displayName,
    this.imageUrl,
    this.size = 44,
    this.heroTag,
    super.key,
  });

  final String displayName;
  final String? imageUrl;
  final double size;
  final Object? heroTag;

  String get _initials {
    final parts = displayName.trim().split(RegExp(r'\s+')).where((p) => p.isNotEmpty).toList();
    if (parts.isEmpty) return '?';
    if (parts.length == 1) return parts.first.substring(0, 1).toUpperCase();
    return (parts.first.substring(0, 1) + parts.last.substring(0, 1)).toUpperCase();
  }

  @override
  Widget build(BuildContext context) {
    final avatar = CircleAvatar(
      radius: size / 2,
      backgroundColor: AppColors.navy.withValues(alpha: 0.1),
      backgroundImage: (imageUrl != null && imageUrl!.isNotEmpty) ? NetworkImage(imageUrl!) : null,
      child: (imageUrl == null || imageUrl!.isEmpty)
          ? Text(
              _initials,
              style: TextStyle(
                color: AppColors.navy,
                fontWeight: FontWeight.w700,
                fontSize: size * 0.38,
              ),
            )
          : null,
    );

    if (heroTag == null) return avatar;
    return Hero(tag: heroTag!, child: avatar);
  }
}
