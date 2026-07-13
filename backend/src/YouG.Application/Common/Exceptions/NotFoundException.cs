namespace YouG.Application.Common.Exceptions;

/// <summary>
/// Maps to HTTP 404 in GlobalExceptionHandler. Also deliberately used for "exists but you're not
/// a member" cases (e.g. a group you don't belong to) to avoid leaking valid resource IDs — see
/// docs/04-API-DESIGN.md Section 1.7.
/// </summary>
public class NotFoundException(string message) : Exception(message);
