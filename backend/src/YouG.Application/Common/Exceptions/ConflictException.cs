namespace YouG.Application.Common.Exceptions;

/// <summary>Maps to HTTP 409 in GlobalExceptionHandler — e.g. registering with an email already in use.</summary>
public class ConflictException(string message) : Exception(message);
