import 'package:flutter/material.dart';

/// A "Continue with {provider}" button matching common modern auth-screen conventions.
/// Falls back to a plain colored initial if the provider's logo asset isn't present yet.
class SocialLoginButton extends StatelessWidget {
  const SocialLoginButton({
    required this.label,
    required this.logoAssetPath,
    required this.fallbackInitial,
    required this.fallbackColor,
    required this.onPressed,
    super.key,
  });

  final String label;
  final String logoAssetPath;
  final String fallbackInitial;
  final Color fallbackColor;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    return OutlinedButton(
      onPressed: onPressed,
      style: OutlinedButton.styleFrom(
        backgroundColor: Colors.white,
        side: BorderSide(color: Colors.grey.shade300),
        foregroundColor: Colors.black87,
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Image.asset(
            logoAssetPath,
            width: 20,
            height: 20,
            errorBuilder: (context, error, stackTrace) => CircleAvatar(
              radius: 10,
              backgroundColor: fallbackColor,
              child: Text(
                fallbackInitial,
                style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w700, color: Colors.white),
              ),
            ),
          ),
          const SizedBox(width: 12),
          Flexible(
            child: Text(
              label,
              style: const TextStyle(fontWeight: FontWeight.w600),
              overflow: TextOverflow.ellipsis,
            ),
          ),
        ],
      ),
    );
  }
}
