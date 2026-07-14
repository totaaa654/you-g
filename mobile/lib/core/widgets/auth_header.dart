import 'package:flutter/material.dart';

import '../theme/app_theme.dart';
import 'mascot.dart';

/// The colored strip at the top of auth screens — back button + mascot, matching the
/// Start screen's branding but compact enough to leave room for the form below.
class AuthHeader extends StatelessWidget {
  const AuthHeader({super.key, this.height = 190});

  final double height;

  @override
  Widget build(BuildContext context) {
    return Container(
      height: height,
      width: double.infinity,
      decoration: const BoxDecoration(gradient: AppColors.backgroundGradient),
      child: SafeArea(
        bottom: false,
        child: Stack(
          children: [
            Positioned(
              left: 4,
              top: 4,
              child: IconButton(
                onPressed: () => Navigator.of(context).maybePop(),
                icon: const Icon(Icons.arrow_back_ios_new_rounded),
                color: AppColors.navy,
                style: IconButton.styleFrom(backgroundColor: Colors.white.withValues(alpha: 0.6)),
              ),
            ),
            const Center(child: Mascot(size: 96)),
          ],
        ),
      ),
    );
  }
}
