import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../auth/presentation/providers/auth_controller.dart';

/// Placeholder landing screen — proves the post-login flow works end-to-end.
/// Replaced by the real Groups/Events home once those features land in mobile.
class HomeScreen extends ConsumerWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final user = ref.watch(authControllerProvider).valueOrNull;

    return Scaffold(
      appBar: AppBar(
        title: const Text('You G?'),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () => ref.read(authControllerProvider.notifier).logout(),
          ),
        ],
      ),
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Welcome, ${user?.displayName ?? ''}!', style: Theme.of(context).textTheme.headlineSmall),
            const SizedBox(height: 8),
            Text('Friend code: ${user?.friendCode ?? ''}'),
          ],
        ),
      ),
    );
  }
}
