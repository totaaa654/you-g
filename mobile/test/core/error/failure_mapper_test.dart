import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:youg/core/error/failure.dart';
import 'package:youg/core/error/failure_mapper.dart';

DioException _exceptionWithStatus(int statusCode, Map<String, dynamic> data) {
  final requestOptions = RequestOptions(path: '/test');
  return DioException(
    requestOptions: requestOptions,
    response: Response(requestOptions: requestOptions, statusCode: statusCode, data: data),
  );
}

void main() {
  group('FailureMapper', () {
    test('maps 400 with field errors to ValidationFailure', () {
      final exception = _exceptionWithStatus(400, {
        'title': 'One or more validation errors occurred.',
        'errors': {
          'password': ['Password must be at least 8 characters.'],
        },
      });

      final failure = FailureMapper.fromDioException(exception);

      expect(failure, isA<ValidationFailure>());
      expect((failure as ValidationFailure).fieldErrors['password'], contains('Password must be at least 8 characters.'));
    });

    test('maps 401 to AuthenticationFailure', () {
      final exception = _exceptionWithStatus(401, {'title': 'Invalid email or password.'});
      expect(FailureMapper.fromDioException(exception), isA<AuthenticationFailure>());
    });

    test('maps 403 to ForbiddenFailure', () {
      final exception = _exceptionWithStatus(403, {'title': 'Only group admins can perform this action.'});
      expect(FailureMapper.fromDioException(exception), isA<ForbiddenFailure>());
    });

    test('maps 404 to NotFoundFailure', () {
      final exception = _exceptionWithStatus(404, {'title': 'Group not found.'});
      expect(FailureMapper.fromDioException(exception), isA<NotFoundFailure>());
    });

    test('maps 409 to ConflictFailure', () {
      final exception = _exceptionWithStatus(409, {'title': 'This event is at capacity.'});
      expect(FailureMapper.fromDioException(exception), isA<ConflictFailure>());
    });

    test('maps a response with no data to UnexpectedFailure with a fallback message', () {
      final exception = _exceptionWithStatus(500, {});
      final failure = FailureMapper.fromDioException(exception);

      expect(failure, isA<UnexpectedFailure>());
      expect(failure.message, 'Something went wrong.');
    });

    test('maps a connection error (no response) to NetworkFailure', () {
      final exception = DioException(requestOptions: RequestOptions(path: '/test'));
      expect(FailureMapper.fromDioException(exception), isA<NetworkFailure>());
    });
  });
}
