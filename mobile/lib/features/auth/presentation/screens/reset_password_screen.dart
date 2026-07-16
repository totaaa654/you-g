import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/error/failure_mapper.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/auth_header.dart';
import '../providers/auth_providers.dart';

class ResetPasswordScreen extends ConsumerStatefulWidget {
  const ResetPasswordScreen({this.email, this.code, super.key});

  /// From the emailed link's `?email=` and `?code=` query parameters, if the screen was reached
  /// that way — used to pre-fill the form. Null when navigating here directly, so both fields
  /// start empty for entering the code by hand (needed when "localhost" in the link doesn't
  /// point at whatever device is running the app — e.g. opening the email on a phone).
  final String? email;
  final String? code;

  @override
  ConsumerState<ResetPasswordScreen> createState() => _ResetPasswordScreenState();
}

class _ResetPasswordScreenState extends ConsumerState<ResetPasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  late final _emailController = TextEditingController(text: widget.email ?? '');
  late final _codeController = TextEditingController(text: widget.code ?? '');
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  bool _isSubmitting = false;
  bool _succeeded = false;

  @override
  void dispose() {
    _emailController.dispose();
    _codeController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isSubmitting = true);
    try {
      await ref.read(authRepositoryProvider).resetPassword(
            email: _emailController.text.trim(),
            code: _codeController.text.trim(),
            newPassword: _passwordController.text,
          );
      if (!mounted) return;
      setState(() {
        _isSubmitting = false;
        _succeeded = true;
      });
    } on DioException catch (e) {
      if (!mounted) return;
      setState(() => _isSubmitting = false);
      final failure = FailureMapper.fromDioException(e);
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: true,
      body: SingleChildScrollView(
        child: Column(
          children: [
            const AuthHeader(height: 160),
            Center(
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 420),
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(24, 32, 24, 24),
                  child: _succeeded ? _buildSuccess(context) : _buildForm(context),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildForm(BuildContext context) {
    final hasPrefilledEmail = widget.email != null;

    return Form(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text('Reset your password', style: Theme.of(context).textTheme.headlineMedium),
          const SizedBox(height: 8),
          Text(
            'Enter the 6-digit code from your email, then choose a new password.',
            style: Theme.of(context).textTheme.bodyMedium,
          ),
          const SizedBox(height: 28),
          if (!hasPrefilledEmail) ...[
            TextFormField(
              controller: _emailController,
              keyboardType: TextInputType.emailAddress,
              decoration: const InputDecoration(
                labelText: 'Email',
                prefixIcon: Icon(Icons.mail_outline_rounded),
              ),
              validator: (value) => (value == null || !value.contains('@')) ? 'Enter a valid email' : null,
            ),
            const SizedBox(height: 14),
          ],
          TextFormField(
            controller: _codeController,
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly, LengthLimitingTextInputFormatter(6)],
            autofocus: !hasPrefilledEmail,
            textAlign: TextAlign.center,
            style: const TextStyle(fontSize: 24, letterSpacing: 12, fontWeight: FontWeight.bold),
            decoration: const InputDecoration(
              labelText: 'Reset code',
              counterText: '',
            ),
            maxLength: 6,
            validator: (value) =>
                (value == null || value.trim().length != 6) ? 'Enter the 6-digit code' : null,
          ),
          const SizedBox(height: 14),
          TextFormField(
            controller: _passwordController,
            obscureText: true,
            decoration: const InputDecoration(
              labelText: 'New password',
              prefixIcon: Icon(Icons.lock_outline_rounded),
            ),
            validator: (value) => (value == null || value.length < 8) ? 'At least 8 characters' : null,
          ),
          const SizedBox(height: 14),
          TextFormField(
            controller: _confirmPasswordController,
            obscureText: true,
            decoration: const InputDecoration(
              labelText: 'Confirm new password',
              prefixIcon: Icon(Icons.lock_outline_rounded),
            ),
            validator: (value) =>
                (value != _passwordController.text) ? "Passwords don't match" : null,
          ),
          const SizedBox(height: 24),
          FilledButton(
            onPressed: _isSubmitting ? null : _submit,
            child: _isSubmitting
                ? const SizedBox(
                    height: 20,
                    width: 20,
                    child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                  )
                : const Text('Reset password'),
          ),
          const SizedBox(height: 16),
          Center(
            child: TextButton(
              onPressed: _isSubmitting ? null : () => context.push('/forgot-password'),
              child: const Text("Didn't get a code? Request a new one"),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSuccess(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Icon(Icons.check_circle_outline_rounded, size: 56, color: AppColors.availableGreen),
        const SizedBox(height: 16),
        Text('Password reset', style: Theme.of(context).textTheme.headlineMedium, textAlign: TextAlign.center),
        const SizedBox(height: 8),
        Text(
          'Your password has been updated. Any other devices signed into this account have been '
          'signed out for your security.',
          style: Theme.of(context).textTheme.bodyMedium,
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 28),
        FilledButton(onPressed: () => context.go('/login'), child: const Text('Log in')),
      ],
    );
  }
}
