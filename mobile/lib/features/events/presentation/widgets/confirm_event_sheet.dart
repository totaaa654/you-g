import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../domain/entities/event_location_option.dart';
import '../../domain/entities/event_time_option.dart';

/// Lets the organizer pick which proposed time/location wins before confirming the event —
/// pre-selects whichever option currently has the most votes.
class ConfirmEventSheet extends StatefulWidget {
  const ConfirmEventSheet({
    required this.timeOptions,
    required this.locationOptions,
    required this.onConfirm,
    super.key,
  });

  final List<EventTimeOption> timeOptions;
  final List<EventLocationOption> locationOptions;
  final void Function(String timeOptionId, String locationOptionId) onConfirm;

  @override
  State<ConfirmEventSheet> createState() => _ConfirmEventSheetState();
}

class _ConfirmEventSheetState extends State<ConfirmEventSheet> {
  late String? _selectedTimeId = _topVoted(widget.timeOptions)?.id;
  late String? _selectedLocationId = _topVoted(widget.locationOptions)?.id;

  static T? _topVoted<T extends Object>(List<T> options) {
    if (options.isEmpty) return null;
    final sorted = [...options]..sort((a, b) {
        final aVotes = (a as dynamic).voteCount as int;
        final bVotes = (b as dynamic).voteCount as int;
        return bVotes.compareTo(aVotes);
      });
    return sorted.first;
  }

  @override
  Widget build(BuildContext context) {
    final canConfirm = _selectedTimeId != null && _selectedLocationId != null;

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text('Confirm event', style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 16),
            Text('Time', style: Theme.of(context).textTheme.titleMedium),
            RadioGroup<String>(
              groupValue: _selectedTimeId,
              onChanged: (value) => setState(() => _selectedTimeId = value),
              child: Column(
                children: [
                  for (final option in widget.timeOptions)
                    RadioListTile<String>(
                      value: option.id,
                      title: Text('${DateFormat('MMM d, h:mm a').format(option.startUtc.toLocal())} (${option.voteCount} votes)'),
                      activeColor: AppColors.navy,
                      contentPadding: EdgeInsets.zero,
                    ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            Text('Location', style: Theme.of(context).textTheme.titleMedium),
            RadioGroup<String>(
              groupValue: _selectedLocationId,
              onChanged: (value) => setState(() => _selectedLocationId = value),
              child: Column(
                children: [
                  for (final option in widget.locationOptions)
                    RadioListTile<String>(
                      value: option.id,
                      title: Text('${option.name} (${option.voteCount} votes)'),
                      activeColor: AppColors.navy,
                      contentPadding: EdgeInsets.zero,
                    ),
                ],
              ),
            ),
            const SizedBox(height: 16),
            AppButton(
              label: 'Confirm event',
              onPressed: canConfirm ? () => widget.onConfirm(_selectedTimeId!, _selectedLocationId!) : null,
            ),
          ],
        ),
      ),
    );
  }
}
