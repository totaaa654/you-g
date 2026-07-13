/// Maps every error the app can encounter to one shape the UI layer switches on,
/// instead of parsing ad hoc Dio/HTTP exceptions per screen.
sealed class Failure {
  const Failure(this.message);

  final String message;
}

class ValidationFailure extends Failure {
  const ValidationFailure(super.message, this.fieldErrors);

  final Map<String, List<String>> fieldErrors;
}

class AuthenticationFailure extends Failure {
  const AuthenticationFailure(super.message);
}

class ForbiddenFailure extends Failure {
  const ForbiddenFailure(super.message);
}

class NotFoundFailure extends Failure {
  const NotFoundFailure(super.message);
}

class ConflictFailure extends Failure {
  const ConflictFailure(super.message);
}

class NetworkFailure extends Failure {
  const NetworkFailure(super.message);
}

class UnexpectedFailure extends Failure {
  const UnexpectedFailure(super.message);
}
