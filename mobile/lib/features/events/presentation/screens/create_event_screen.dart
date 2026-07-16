import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../groups/presentation/providers/groups_providers.dart';
import '../providers/events_providers.dart';

TimeOfDay _parseStartTime(String? startTime) {
  if (startTime == null) return const TimeOfDay(hour: 9, minute: 0);
  final parts = startTime.split(':');
  return TimeOfDay(hour: int.parse(parts[0]), minute: int.parse(parts[1]));
}

class CreateEventScreen extends ConsumerStatefulWidget {
  const CreateEventScreen({required this.groupId, this.initialDate, this.initialStartTime, super.key});

  final String groupId;
  final DateTime? initialDate;
  final String? initialStartTime;

  @override
  ConsumerState<CreateEventScreen> createState() => _CreateEventScreenState();
}

class _CreateEventScreenState extends ConsumerState<CreateEventScreen> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _locationNameController = TextEditingController();
  final _locationAddressController = TextEditingController();
  final _maxAttendeesController = TextEditingController();

  late DateTime _date = widget.initialDate ?? DateTime.now().add(const Duration(days: 1));
  late TimeOfDay _startTime = _parseStartTime(widget.initialStartTime);
  late TimeOfDay _endTime = TimeOfDay(hour: (_startTime.hour + 2) % 24, minute: _startTime.minute);

  bool _submitting = false;

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    _locationNameController.dispose();
    _locationAddressController.dispose();
    _maxAttendeesController.dispose();
    super.dispose();
  }

  DateTime _combine(TimeOfDay time) => DateTime(_date.year, _date.month, _date.day, time.hour, time.minute);

  Future<void> _pickDate() async {
    final picked = await showDatePicker(
      context: context,
      initialDate: _date,
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (picked != null) setState(() => _date = picked);
  }

  Future<void> _pickTime({required bool isStart}) async {
    final picked = await showTimePicker(context: context, initialTime: isStart ? _startTime : _endTime);
    if (picked == null) return;
    setState(() => isStart ? _startTime = picked : _endTime = picked);
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _submitting = true);

    try {
      final event = await ref.read(eventsRepositoryProvider).createEvent(
            groupId: widget.groupId,
            title: _titleController.text.trim(),
            description: _descriptionController.text.trim().isEmpty ? null : _descriptionController.text.trim(),
            maxAttendees: int.tryParse(_maxAttendeesController.text.trim()),
            initialStartUtc: _combine(_startTime),
            initialEndUtc: _combine(_endTime),
            initialLocationName: _locationNameController.text.trim().isEmpty ? null : _locationNameController.text.trim(),
            initialLocationAddress: _locationAddressController.text.trim().isEmpty ? null : _locationAddressController.text.trim(),
            // No map picker yet (see the placeholder below) - the backend only requires
            // coordinates alongside a location name, so this is a honest placeholder value
            // until a real picker exists, not a real "location".
            initialLocationLatitude: _locationNameController.text.trim().isEmpty ? null : 0,
            initialLocationLongitude: _locationNameController.text.trim().isEmpty ? null : 0,
          );
      ref.invalidate(groupEventsProvider(widget.groupId));
      if (!mounted) return;
      context.pushReplacement('/events/${event.id}');
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text("Couldn't create the event. Try again.")));
    } finally {
      if (mounted) setState(() => _submitting = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final groupAsync = ref.watch(groupByIdProvider(widget.groupId));

    return Scaffold(
      appBar: AppBar(title: const Text('Create event')),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(16),
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                TextFormField(
                  controller: _titleController,
                  decoration: const InputDecoration(labelText: 'Title'),
                  validator: (v) => (v == null || v.trim().isEmpty) ? 'Enter a title' : null,
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _descriptionController,
                  decoration: const InputDecoration(labelText: 'Description (optional)'),
                  maxLines: 3,
                ),
                const SizedBox(height: 20),
                Row(
                  children: [
                    Expanded(
                      child: _PickerField(
                        label: 'Date',
                        value: DateFormat('EEE, MMM d').format(_date),
                        icon: Icons.calendar_today_rounded,
                        onTap: _pickDate,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                Row(
                  children: [
                    Expanded(
                      child: _PickerField(
                        label: 'Start time',
                        value: _startTime.format(context),
                        icon: Icons.schedule_rounded,
                        onTap: () => _pickTime(isStart: true),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: _PickerField(
                        label: 'End time',
                        value: _endTime.format(context),
                        icon: Icons.schedule_rounded,
                        onTap: () => _pickTime(isStart: false),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                TextFormField(
                  controller: _maxAttendeesController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(labelText: 'Maximum participants (optional)'),
                ),
                const SizedBox(height: 20),
                Text('Location', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 10),
                // Manual entry, not a live picker — matches the OpenStreetMap-over-Google-Maps
                // decision (no interactive map wired yet, this is the placeholder).
                Container(
                  height: 110,
                  decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(16)),
                  alignment: Alignment.center,
                  child: const Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.map_outlined, color: AppColors.unknownGray, size: 28),
                      SizedBox(height: 6),
                      Text('Map preview coming soon', style: TextStyle(color: AppColors.unknownGray, fontSize: 12)),
                    ],
                  ),
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _locationNameController,
                  decoration: const InputDecoration(labelText: 'Location name (optional)'),
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _locationAddressController,
                  decoration: const InputDecoration(labelText: 'Address (optional)'),
                ),
                const SizedBox(height: 20),
                groupAsync.when(
                  loading: () => const SizedBox.shrink(),
                  error: (_, _) => const SizedBox.shrink(),
                  data: (group) => Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(14)),
                    child: Row(
                      children: [
                        const Icon(Icons.info_outline_rounded, color: AppColors.accentBlue, size: 20),
                        const SizedBox(width: 10),
                        Expanded(
                          child: Text(
                            'Everyone in ${group.name} will be able to see and join this event.',
                            style: const TextStyle(fontSize: 12.5, color: AppColors.navy),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(height: 24),
                AppButton(label: 'Create event', isLoading: _submitting, onPressed: _submit),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _PickerField extends StatelessWidget {
  const _PickerField({required this.label, required this.value, required this.icon, required this.onTap});

  final String label;
  final String value;
  final IconData icon;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
        decoration: BoxDecoration(color: AppColors.fog, borderRadius: BorderRadius.circular(16)),
        child: Row(
          children: [
            Icon(icon, size: 18, color: AppColors.navy.withValues(alpha: 0.6)),
            const SizedBox(width: 10),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(label, style: const TextStyle(fontSize: 11, color: AppColors.unknownGray)),
                  Text(value, style: const TextStyle(fontWeight: FontWeight.w600)),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
