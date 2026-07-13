namespace YouG.Application.Common.Exceptions;

/// <summary>Maps to HTTP 401 in GlobalExceptionHandler — e.g. invalid credentials or an expired/revoked refresh token.</summary>
public class AuthenticationFailedException(string message) : Exception(message);
