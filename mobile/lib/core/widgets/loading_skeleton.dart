import 'package:flutter/material.dart';

import '../theme/app_theme.dart';

/// A single shimmering placeholder block. Compose several inside a `Column`/`ListView` to
/// build a screen-shaped skeleton while real data loads — no external shimmer package needed,
/// just a self-contained sweeping gradient.
class LoadingSkeleton extends StatefulWidget {
  const LoadingSkeleton({
    this.width = double.infinity,
    this.height = 16,
    this.borderRadius = 10,
    super.key,
  });

  final double width;
  final double height;
  final double borderRadius;

  @override
  State<LoadingSkeleton> createState() => _LoadingSkeletonState();
}

class _LoadingSkeletonState extends State<LoadingSkeleton> with SingleTickerProviderStateMixin {
  late final AnimationController _controller =
      AnimationController(vsync: this, duration: const Duration(milliseconds: 1200))..repeat();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _controller,
      builder: (context, child) {
        final t = _controller.value;
        return ShaderMask(
          blendMode: BlendMode.srcATop,
          shaderCallback: (bounds) => LinearGradient(
            begin: Alignment(-1 + 3 * t, 0),
            end: Alignment(1 + 3 * t, 0),
            colors: [AppColors.fog, Colors.white, AppColors.fog],
            stops: const [0.35, 0.5, 0.65],
          ).createShader(bounds),
          child: Container(
            width: widget.width,
            height: widget.height,
            decoration: BoxDecoration(
              color: AppColors.fog,
              borderRadius: BorderRadius.circular(widget.borderRadius),
            ),
          ),
        );
      },
    );
  }
}
