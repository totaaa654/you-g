import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/loading_skeleton.dart';
import '../../../auth/presentation/providers/auth_controller.dart';
import '../../../profile/domain/entities/profile.dart';
import '../../../profile/domain/entities/settings.dart';
import '../../../profile/domain/entities/theme_preference.dart';
import '../../../profile/presentation/providers/profile_providers.dart';

class SettingsScreen extends ConsumerWidget {
  const SettingsScreen({super.key});

  Future<void> _applySettings(WidgetRef ref, Settings current, Settings Function(Settings) update) {
    final next = update(current);
    return ref.read(myProfileProvider.notifier).updateSettings(
          themePreference: next.themePreference,
          isSearchable: next.isSearchable,
          notifyOnFriendRequest: next.notifyOnFriendRequest,
          notifyOnGroupInvite: next.notifyOnGroupInvite,
          notifyOnEventReminder: next.notifyOnEventReminder,
          notifyOnScheduleUpdate: next.notifyOnScheduleUpdate,
        );
  }

  Future<void> _confirmDeleteAccount(BuildContext context, WidgetRef ref) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete account?'),
        content: const Text('This permanently deletes your account. This action cannot be undone.'),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: Text('Delete', style: TextStyle(color: Theme.of(context).colorScheme.error)),
          ),
        ],
      ),
    );
    if (confirmed != true) return;

    await ref.read(profileRepositoryProvider).deleteMyAccount();
    await ref.read(authControllerProvider.notifier).logout();
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final profileAsync = ref.watch(myProfileProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Settings')),
      body: profileAsync.when(
        loading: () => const Padding(padding: EdgeInsets.all(16), child: LoadingSkeleton(height: 300, borderRadius: 20)),
        error: (_, _) => const Center(child: Text("Couldn't load your settings.")),
        data: (Profile profile) {
          final settings = profile.settings;

          return ListView(
            padding: const EdgeInsets.symmetric(vertical: 8),
            children: [
              const _SectionHeader('Appearance'),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: SegmentedButton<ThemePreference>(
                  segments: ThemePreference.values
                      .map((mode) => ButtonSegment(value: mode, label: Text(mode.label)))
                      .toList(),
                  selected: {settings.themePreference},
                  onSelectionChanged: (selection) =>
                      _applySettings(ref, settings, (s) => s.copyWith(themePreference: selection.first)),
                ),
              ),
              const _SectionHeader('Notifications'),
              SwitchListTile(
                title: const Text('Friend requests'),
                value: settings.notifyOnFriendRequest,
                activeThumbColor: AppColors.navy,
                onChanged: (v) => _applySettings(ref, settings, (s) => s.copyWith(notifyOnFriendRequest: v)),
              ),
              SwitchListTile(
                title: const Text('Group invites'),
                value: settings.notifyOnGroupInvite,
                activeThumbColor: AppColors.navy,
                onChanged: (v) => _applySettings(ref, settings, (s) => s.copyWith(notifyOnGroupInvite: v)),
              ),
              SwitchListTile(
                title: const Text('Event reminders'),
                value: settings.notifyOnEventReminder,
                activeThumbColor: AppColors.navy,
                onChanged: (v) => _applySettings(ref, settings, (s) => s.copyWith(notifyOnEventReminder: v)),
              ),
              SwitchListTile(
                title: const Text('Schedule updates'),
                value: settings.notifyOnScheduleUpdate,
                activeThumbColor: AppColors.navy,
                onChanged: (v) => _applySettings(ref, settings, (s) => s.copyWith(notifyOnScheduleUpdate: v)),
              ),
              const _SectionHeader('Privacy'),
              SwitchListTile(
                title: const Text('Show me in search'),
                subtitle: const Text('Friends and group members can always find you regardless'),
                value: settings.isSearchable,
                activeThumbColor: AppColors.navy,
                onChanged: (v) => _applySettings(ref, settings, (s) => s.copyWith(isSearchable: v)),
              ),
              ListTile(
                title: const Text('Blocked users'),
                trailing: const Icon(Icons.chevron_right_rounded),
                onTap: () => ScaffoldMessenger.of(context)
                    .showSnackBar(const SnackBar(content: Text('Managing blocked users is coming soon.'))),
              ),
              const _SectionHeader('Calendar'),
              SwitchListTile(
                title: const Text('Sync with external calendar'),
                subtitle: const Text('Coming in a future update'),
                value: false,
                onChanged: null,
              ),
              const _SectionHeader('Account'),
              ListTile(
                title: const Text('About'),
                trailing: const Icon(Icons.chevron_right_rounded),
                onTap: () => showAboutDialog(context: context, applicationName: 'You G?', applicationVersion: '1.0.0'),
              ),
              ListTile(
                title: const Text('Log out'),
                leading: const Icon(Icons.logout_rounded),
                onTap: () => ref.read(authControllerProvider.notifier).logout(),
              ),
              ListTile(
                title: Text('Delete account', style: TextStyle(color: Theme.of(context).colorScheme.error)),
                leading: Icon(Icons.delete_outline_rounded, color: Theme.of(context).colorScheme.error),
                onTap: () => _confirmDeleteAccount(context, ref),
              ),
              const SizedBox(height: 24),
            ],
          );
        },
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  const _SectionHeader(this.title);

  final String title;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 20, 16, 6),
      child: Text(
        title.toUpperCase(),
        style: const TextStyle(fontSize: 11.5, fontWeight: FontWeight.w700, color: AppColors.unknownGray, letterSpacing: 0.4),
      ),
    );
  }
}
