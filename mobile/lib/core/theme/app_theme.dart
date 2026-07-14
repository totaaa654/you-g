import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';

/// Brand palette — a friendly navy-and-gold scheme built around the calendar mascot's colors.
/// Primary background is always white; gold is reserved for highlights/CTAs, navy carries
/// icons/text/navigation — post-login screens lean on `primaryNavy`/`accentBlue`/`fog` for a
/// calmer, content-first feel than the auth flow's gradient branding moment.
class AppColors {
  static const primaryNavy = Color(0xFF12233A);
  static const navy = Color(0xFF1E3A5F); // "Secondary Navy"
  static const accentBlue = Color(0xFF3A5C86);
  static const gold = Color(0xFFFFC94D);
  static const cream = Color(0xFFFFFBF2);
  static const fog = Color(0xFFEEF1F5);
  static const fieldFill = fog;

  // Availability status — semantic, deliberately separate from the brand accent (gold is
  // reserved for CTAs/highlights, so "Maybe" borrows it only because that's the one semantic
  // state that's inherently gold-coded across the whole product, e.g. AvailabilityStatus.Maybe).
  static const availableGreen = Color(0xFF2FA36E);
  static const busyRed = Color(0xFFE0564B);
  static const maybeGold = gold;
  static const unknownGray = Color(0xFF9AA6B4);

  /// Top-to-bottom gradient used behind the Start screen and the header strip on auth screens.
  static const backgroundGradient = LinearGradient(
    begin: Alignment.topCenter,
    end: Alignment.bottomCenter,
    colors: [cream, gold],
  );
}

class AppTheme {
  static const _buttonRadius = 16.0;

  static ThemeData light() {
    final colorScheme = ColorScheme.fromSeed(
      seedColor: AppColors.navy,
      primary: AppColors.navy,
      secondary: AppColors.gold,
      surface: Colors.white,
    );

    return ThemeData(
      useMaterial3: true,
      colorScheme: colorScheme,
      scaffoldBackgroundColor: Colors.white,
      textTheme: _textTheme(),
      appBarTheme: const AppBarTheme(
        backgroundColor: Colors.transparent,
        foregroundColor: AppColors.navy,
        elevation: 0,
        scrolledUnderElevation: 0,
        titleTextStyle: TextStyle(color: AppColors.primaryNavy, fontSize: 18, fontWeight: FontWeight.w700),
        iconTheme: IconThemeData(color: AppColors.navy),
      ),
      cardTheme: CardThemeData(
        color: Colors.white,
        elevation: 0,
        margin: EdgeInsets.zero,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(20),
          side: BorderSide(color: AppColors.fog, width: 1),
        ),
      ),
      dividerTheme: DividerThemeData(color: AppColors.fog, thickness: 1, space: 1),
      bottomNavigationBarTheme: const BottomNavigationBarThemeData(
        backgroundColor: Colors.white,
        selectedItemColor: AppColors.navy,
        unselectedItemColor: Color(0xFFB7C0CC),
        type: BottomNavigationBarType.fixed,
        elevation: 0,
        selectedLabelStyle: TextStyle(fontSize: 11, fontWeight: FontWeight.w700),
        unselectedLabelStyle: TextStyle(fontSize: 11, fontWeight: FontWeight.w600),
      ),
      floatingActionButtonTheme: const FloatingActionButtonThemeData(
        backgroundColor: AppColors.navy,
        foregroundColor: Colors.white,
      ),
      pageTransitionsTheme: const PageTransitionsTheme(
        builders: {
          TargetPlatform.android: CupertinoPageTransitionsBuilder(),
          TargetPlatform.iOS: CupertinoPageTransitionsBuilder(),
          TargetPlatform.windows: CupertinoPageTransitionsBuilder(),
          TargetPlatform.macOS: CupertinoPageTransitionsBuilder(),
          TargetPlatform.linux: CupertinoPageTransitionsBuilder(),
        },
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          backgroundColor: AppColors.navy,
          foregroundColor: Colors.white,
          minimumSize: const Size.fromHeight(54),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(_buttonRadius)),
          textStyle: GoogleFonts.manrope(fontSize: 16, fontWeight: FontWeight.w700),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: AppColors.navy,
          minimumSize: const Size.fromHeight(54),
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(_buttonRadius)),
          side: const BorderSide(color: AppColors.navy, width: 1.5),
          textStyle: GoogleFonts.manrope(fontSize: 16, fontWeight: FontWeight.w700),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.navy,
          textStyle: GoogleFonts.manrope(fontWeight: FontWeight.w600),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppColors.fieldFill,
        contentPadding: const EdgeInsets.symmetric(horizontal: 18, vertical: 16),
        prefixIconColor: AppColors.navy.withValues(alpha: 0.55),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(_buttonRadius),
          borderSide: BorderSide.none,
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(_buttonRadius),
          borderSide: BorderSide.none,
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(_buttonRadius),
          borderSide: const BorderSide(color: AppColors.navy, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(_buttonRadius),
          borderSide: BorderSide(color: colorScheme.error, width: 1.5),
        ),
        labelStyle: TextStyle(color: AppColors.navy.withValues(alpha: 0.7)),
      ),
    );
  }

  static ThemeData dark() => ThemeData(
        useMaterial3: true,
        colorScheme: ColorScheme.fromSeed(
          seedColor: AppColors.navy,
          secondary: AppColors.gold,
          brightness: Brightness.dark,
        ),
      );

  static TextTheme _textTheme() {
    final base = GoogleFonts.manropeTextTheme();
    return base.copyWith(
      headlineLarge: base.headlineLarge?.copyWith(color: AppColors.navy, fontWeight: FontWeight.w800),
      headlineMedium: base.headlineMedium?.copyWith(color: AppColors.navy, fontWeight: FontWeight.w800),
      headlineSmall: base.headlineSmall?.copyWith(color: AppColors.navy, fontWeight: FontWeight.w700),
      titleLarge: base.titleLarge?.copyWith(color: AppColors.navy, fontWeight: FontWeight.w700),
      titleMedium: base.titleMedium?.copyWith(color: AppColors.navy, fontWeight: FontWeight.w600),
      bodyLarge: base.bodyLarge?.copyWith(color: AppColors.navy.withValues(alpha: 0.7)),
      bodyMedium: base.bodyMedium?.copyWith(color: AppColors.navy.withValues(alpha: 0.7)),
    );
  }
}
