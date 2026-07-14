import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

/// The 5-tab bottom navigation shell (Home/Friends/Groups/Calendar/Profile). Wraps GoRouter's
/// `StatefulNavigationShell` so each tab keeps its own navigation stack/scroll position when
/// switching — matches the Notion/Discord persistent-tab feel called for in the design brief.
class BottomNavShell extends StatelessWidget {
  const BottomNavShell({required this.navigationShell, super.key});

  final StatefulNavigationShell navigationShell;

  static const _items = [
    (icon: Icons.home_outlined, activeIcon: Icons.home_rounded, label: 'Home'),
    (icon: Icons.people_outline_rounded, activeIcon: Icons.people_rounded, label: 'Friends'),
    (icon: Icons.groups_outlined, activeIcon: Icons.groups_rounded, label: 'Groups'),
    (icon: Icons.calendar_today_outlined, activeIcon: Icons.calendar_today_rounded, label: 'Calendar'),
    (icon: Icons.person_outline_rounded, activeIcon: Icons.person_rounded, label: 'Profile'),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: navigationShell,
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: navigationShell.currentIndex,
        onTap: (index) => navigationShell.goBranch(
          index,
          // Tapping the already-active tab pops it back to its root, matching standard
          // bottom-nav behavior (Instagram/Discord) instead of leaving a stale pushed screen.
          initialLocation: index == navigationShell.currentIndex,
        ),
        items: [
          for (final item in _items)
            BottomNavigationBarItem(icon: Icon(item.icon), activeIcon: Icon(item.activeIcon), label: item.label),
        ],
      ),
    );
  }
}
