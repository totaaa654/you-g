import 'package:flutter/material.dart';

import '../theme/app_theme.dart';

/// The calendar mascot — falls back to a placeholder icon if assets/images/mascot.png
/// hasn't been added yet, so the app still builds and runs without it.
class Mascot extends StatelessWidget {
  const Mascot({super.key, this.size = 220});

  final double size;

  @override
  Widget build(BuildContext context) {
    return Image.asset(
      'assets/images/mascot.png',
      width: size,
      height: size,
      errorBuilder: (context, error, stackTrace) => Container(
        width: size,
        height: size,
        decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(size * 0.18)),
        child: Icon(Icons.calendar_month_rounded, size: size * 0.55, color: AppColors.navy),
      ),
    );
  }
}
