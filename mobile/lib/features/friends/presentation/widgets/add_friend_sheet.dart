import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/profile_avatar.dart';
import '../providers/friends_providers.dart';

/// Two ways to add a friend, matching the backend contract: search by username (debounced,
/// live results) or paste an exact friend code (`YG-XXXXXX`) and send directly.
class AddFriendSheet extends ConsumerStatefulWidget {
  const AddFriendSheet({super.key});

  @override
  ConsumerState<AddFriendSheet> createState() => _AddFriendSheetState();
}

class _AddFriendSheetState extends ConsumerState<AddFriendSheet> with SingleTickerProviderStateMixin {
  late final TabController _tabController = TabController(length: 2, vsync: this);
  final _searchController = TextEditingController();
  final _friendCodeController = TextEditingController();
  String _query = '';
  bool _sending = false;
  final Set<String> _sentTo = {};

  @override
  void dispose() {
    _tabController.dispose();
    _searchController.dispose();
    _friendCodeController.dispose();
    super.dispose();
  }

  Future<void> _sendRequest({String? userId, String? friendCode}) async {
    setState(() => _sending = true);
    try {
      await ref.read(friendsRepositoryProvider).sendFriendRequest(addresseeId: userId, friendCode: friendCode);
      if (userId != null) setState(() => _sentTo.add(userId));
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Friend request sent.')));
      if (friendCode != null) _friendCodeController.clear();
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context)
          .showSnackBar(const SnackBar(content: Text("Couldn't send that request. Check the details and try again.")));
    } finally {
      if (mounted) setState(() => _sending = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      initialChildSize: 0.75,
      minChildSize: 0.5,
      maxChildSize: 0.95,
      expand: false,
      builder: (context, scrollController) => Column(
        children: [
          const SizedBox(height: 12),
          Container(width: 40, height: 4, decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(999))),
          const SizedBox(height: 16),
          Text('Add a friend', style: Theme.of(context).textTheme.titleLarge),
          TabBar(
            controller: _tabController,
            labelColor: AppColors.navy,
            unselectedLabelColor: AppColors.unknownGray,
            indicatorColor: AppColors.navy,
            tabs: const [Tab(text: 'Search'), Tab(text: 'Friend code')],
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                _buildSearchTab(scrollController),
                _buildFriendCodeTab(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchTab(ScrollController scrollController) {
    final results = ref.watch(userSearchProvider(_query));

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16),
          child: TextField(
            controller: _searchController,
            autofocus: true,
            decoration: const InputDecoration(hintText: 'Search by username', prefixIcon: Icon(Icons.search_rounded)),
            onChanged: (value) => setState(() => _query = value),
          ),
        ),
        Expanded(
          child: results.when(
            loading: () => const Center(child: CircularProgressIndicator()),
            error: (_, _) => const Center(child: Text('Search failed. Try again.')),
            data: (users) {
              if (_query.trim().isEmpty) {
                return const Center(child: Text('Start typing a username to search.'));
              }
              if (users.isEmpty) {
                return const Center(child: Text('No users found.'));
              }
              return ListView.builder(
                controller: scrollController,
                padding: const EdgeInsets.symmetric(horizontal: 16),
                itemCount: users.length,
                itemBuilder: (context, index) {
                  final user = users[index];
                  final alreadySent = _sentTo.contains(user.id);
                  return ListTile(
                    leading: ProfileAvatar(displayName: user.displayName, imageUrl: user.profilePictureUrl),
                    title: Text(user.displayName),
                    subtitle: Text('@${user.username}'),
                    trailing: alreadySent
                        ? const Icon(Icons.check_circle_rounded, color: AppColors.availableGreen)
                        : TextButton(
                            onPressed: _sending ? null : () => _sendRequest(userId: user.id),
                            child: const Text('Add'),
                          ),
                  );
                },
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildFriendCodeTab() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          TextField(
            controller: _friendCodeController,
            textCapitalization: TextCapitalization.characters,
            decoration: const InputDecoration(hintText: 'YG-XXXXXX', prefixIcon: Icon(Icons.tag_rounded)),
          ),
          const SizedBox(height: 16),
          AppButton(
            label: 'Send request',
            isLoading: _sending,
            onPressed: _friendCodeController.text.trim().isEmpty
                ? null
                : () => _sendRequest(friendCode: _friendCodeController.text.trim()),
          ),
        ],
      ),
    );
  }
}
