import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/groups/presentation/providers/groups_providers.dart';

/// The 5-tab bottom navigation shell (Home/Friends/Groups/Calendar/Profile). Wraps GoRouter's
/// `StatefulNavigationShell` so each tab keeps its own navigation stack/scroll position when
/// switching — matches the Notion/Discord persistent-tab feel called for in the design brief.
class BottomNavShell extends ConsumerStatefulWidget {
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
  ConsumerState<BottomNavShell> createState() => _BottomNavShellState();
}

class _BottomNavShellState extends ConsumerState<BottomNavShell> with WidgetsBindingObserver {
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    // Real push (SignalR) is deliberately deferred — see docs/ROADMAP.md's decision log. This
    // is the lightweight stand-in so e.g. a group join request being accepted by an admin shows
    // up for the requester without them needing to manually refresh or restart the app.
    _pollTimer = Timer.periodic(const Duration(seconds: 20), (_) => _refreshMyGroups());
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) _refreshMyGroups();
  }

  void _refreshMyGroups() => ref.read(myGroupsProvider.notifier).refresh();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: widget.navigationShell,
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: widget.navigationShell.currentIndex,
        onTap: (index) => widget.navigationShell.goBranch(
          index,
          // Tapping the already-active tab pops it back to its root, matching standard
          // bottom-nav behavior (Instagram/Discord) instead of leaving a stale pushed screen.
          initialLocation: index == widget.navigationShell.currentIndex,
        ),
        items: [
          for (final item in BottomNavShell._items)
            BottomNavigationBarItem(icon: Icon(item.icon), activeIcon: Icon(item.activeIcon), label: item.label),
        ],
      ),
    );
  }
}
