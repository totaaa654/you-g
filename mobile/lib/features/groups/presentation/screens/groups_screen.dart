import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/widgets/app_search_bar.dart';
import '../../../../core/widgets/empty_state.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../widgets/create_or_join_group_sheet.dart';
import '../widgets/group_tile.dart';
import '../providers/groups_providers.dart';

class GroupsScreen extends ConsumerStatefulWidget {
  const GroupsScreen({super.key});

  @override
  ConsumerState<GroupsScreen> createState() => _GroupsScreenState();
}

class _GroupsScreenState extends ConsumerState<GroupsScreen> {
  String _query = '';

  void _openCreateOrJoinSheet() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
      builder: (context) => const CreateOrJoinGroupSheet(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final groupsAsync = ref.watch(myGroupsProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Groups'),
        actions: [IconButton(onPressed: _openCreateOrJoinSheet, icon: const Icon(Icons.add_rounded))],
      ),
      body: RefreshIndicator(
        onRefresh: () => ref.read(myGroupsProvider.notifier).refresh(),
        child: groupsAsync.when(
          loading: () => ListView(
            padding: const EdgeInsets.all(16),
            children: const [
              LoadingSkeleton(height: 76, borderRadius: 20),
              SizedBox(height: 12),
              LoadingSkeleton(height: 76, borderRadius: 20),
            ],
          ),
          error: (_, _) => ListView(
            children: const [
              EmptyState(icon: Icons.error_outline_rounded, title: "Couldn't load your groups", message: 'Pull down to try again.'),
            ],
          ),
          data: (groups) {
            if (groups.isEmpty) {
              return ListView(
                children: [
                  EmptyState(
                    icon: Icons.groups_outlined,
                    title: 'No groups yet',
                    message: 'Create a group or join one with an invite code.',
                    actionLabel: 'Create or join a group',
                    onAction: _openCreateOrJoinSheet,
                  ),
                ],
              );
            }

            final query = _query.trim().toLowerCase();
            final filtered =
                query.isEmpty ? groups : groups.where((g) => g.name.toLowerCase().contains(query)).toList();

            return ListView(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
              children: [
                AppSearchBar(hintText: 'Search groups', onChanged: (v) => setState(() => _query = v)),
                const SizedBox(height: 16),
                if (filtered.isEmpty)
                  const Padding(
                    padding: EdgeInsets.symmetric(vertical: 24),
                    child: Center(child: Text('No groups match your search.')),
                  )
                else
                  for (final group in filtered) ...[
                    GroupTile(group: group, onTap: () => context.push('/groups/${group.id}')),
                    const SizedBox(height: 10),
                  ],
              ],
            );
          },
        ),
      ),
    );
  }
}
