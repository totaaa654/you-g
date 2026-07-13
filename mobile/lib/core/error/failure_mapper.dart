import 'package:dio/dio.dart';

import 'failure.dart';

/// Reads the backend's RFC 7807 ProblemDetails shape (docs/04-API-DESIGN.md Section 1.5)
/// and turns it into a typed Failure the UI can switch on.
class FailureMapper {
  static Failure fromDioException(DioException exception) {
    final response = exception.response;

    if (response == null) {
      return const NetworkFailure('Could not reach the server. Check your connection.');
    }

    final data = response.data;
    final title = (data is Map && data['title'] is String) ? data['title'] as String : 'Something went wrong.';

    switch (response.statusCode) {
      case 400:
        final errorsRaw = (data is Map) ? data['errors'] : null;
        final fieldErrors = <String, List<String>>{};
        if (errorsRaw is Map) {
          for (final entry in errorsRaw.entries) {
            fieldErrors[entry.key.toString()] = (entry.value as List).map((e) => e.toString()).toList();
          }
        }
        return ValidationFailure(title, fieldErrors);
      case 401:
        return AuthenticationFailure(title);
      case 403:
        return ForbiddenFailure(title);
      case 404:
        return NotFoundFailure(title);
      case 409:
        return ConflictFailure(title);
      default:
        return UnexpectedFailure(title);
    }
  }
}
