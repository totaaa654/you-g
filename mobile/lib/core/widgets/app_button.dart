import 'package:flutter/material.dart';

enum AppButtonVariant { primary, secondary, text }

/// Thin convenience wrapper over the themed Filled/Outlined/Text buttons — standardizes the
/// loading-spinner-replaces-label pattern (previously duplicated per screen) and optional
/// leading icon, without overriding the look those buttons already get from `AppTheme`.
class AppButton extends StatelessWidget {
  const AppButton({
    required this.label,
    required this.onPressed,
    this.variant = AppButtonVariant.primary,
    this.isLoading = false,
    this.icon,
    super.key,
  });

  final String label;
  final VoidCallback? onPressed;
  final AppButtonVariant variant;
  final bool isLoading;
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    final child = isLoading
        ? SizedBox(
            height: 20,
            width: 20,
            child: CircularProgressIndicator(
              strokeWidth: 2,
              color: variant == AppButtonVariant.primary ? Colors.white : null,
            ),
          )
        : icon == null
            ? Text(label)
            : Row(
                mainAxisSize: MainAxisSize.min,
                children: [Icon(icon, size: 18), const SizedBox(width: 8), Text(label)],
              );

    final effectiveOnPressed = isLoading ? null : onPressed;

    return switch (variant) {
      AppButtonVariant.primary => FilledButton(onPressed: effectiveOnPressed, child: child),
      AppButtonVariant.secondary => OutlinedButton(onPressed: effectiveOnPressed, child: child),
      AppButtonVariant.text => TextButton(onPressed: effectiveOnPressed, child: child),
    };
  }
}
