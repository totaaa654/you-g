import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../providers/groups_providers.dart';

/// One sheet, two tabs — matches the `AddFriendSheet` pattern (search-vs-code style dual entry
/// point) so creating a group and joining one via invite code feel like the same interaction.
class CreateOrJoinGroupSheet extends ConsumerStatefulWidget {
  const CreateOrJoinGroupSheet({super.key});

  @override
  ConsumerState<CreateOrJoinGroupSheet> createState() => _CreateOrJoinGroupSheetState();
}

class _CreateOrJoinGroupSheetState extends ConsumerState<CreateOrJoinGroupSheet>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController = TabController(length: 2, vsync: this);
  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _codeController = TextEditingController();
  bool _busy = false;
  String? _error;

  @override
  void dispose() {
    _tabController.dispose();
    _nameController.dispose();
    _descriptionController.dispose();
    _codeController.dispose();
    super.dispose();
  }

  Future<void> _createGroup() async {
    if (_nameController.text.trim().isEmpty) return;
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      final group = await ref.read(myGroupsProvider.notifier).createGroup(
            name: _nameController.text.trim(),
            description: _descriptionController.text.trim().isEmpty ? null : _descriptionController.text.trim(),
          );
      if (!mounted) return;
      Navigator.of(context).pop();
      context.push('/groups/${group.id}');
    } catch (_) {
      setState(() => _error = "Couldn't create the group. Try again.");
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _joinGroup() async {
    if (_codeController.text.trim().isEmpty) return;
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      final result = await ref.read(myGroupsProvider.notifier).joinByInviteCode(_codeController.text.trim());
      if (!mounted) return;
      Navigator.of(context).pop();
      if (!result.joined) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Request sent — an admin needs to approve it before you join.')),
        );
      }
    } catch (_) {
      setState(() => _error = "That invite code didn't work. Double-check it and try again.");
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(bottom: MediaQuery.of(context).viewInsets.bottom),
      child: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(20),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Center(
                child: Container(
                  width: 40,
                  height: 4,
                  decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(999)),
                ),
              ),
              const SizedBox(height: 16),
              TabBar(
                controller: _tabController,
                labelColor: AppColors.navy,
                unselectedLabelColor: AppColors.unknownGray,
                indicatorColor: AppColors.navy,
                tabs: const [Tab(text: 'Create group'), Tab(text: 'Join with code')],
              ),
              const SizedBox(height: 16),
              SizedBox(
                height: 220,
                child: TabBarView(
                  controller: _tabController,
                  children: [
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        TextField(controller: _nameController, decoration: const InputDecoration(labelText: 'Group name')),
                        const SizedBox(height: 12),
                        TextField(
                          controller: _descriptionController,
                          decoration: const InputDecoration(labelText: 'Description (optional)'),
                        ),
                        const Spacer(),
                        AppButton(label: 'Create', isLoading: _busy, onPressed: _createGroup),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        TextField(
                          controller: _codeController,
                          textCapitalization: TextCapitalization.characters,
                          decoration: const InputDecoration(labelText: 'Invite code', prefixIcon: Icon(Icons.tag_rounded)),
                        ),
                        const Spacer(),
                        AppButton(label: 'Join group', isLoading: _busy, onPressed: _joinGroup),
                      ],
                    ),
                  ],
                ),
              ),
              if (_error != null) ...[
                const SizedBox(height: 8),
                Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
              ],
            ],
          ),
        ),
      ),
    );
  }
}
