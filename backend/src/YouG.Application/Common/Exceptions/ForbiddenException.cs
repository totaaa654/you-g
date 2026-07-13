namespace YouG.Application.Common.Exceptions;

/// <summary>
/// Maps to HTTP 403 in GlobalExceptionHandler. Reserved for cases where the resource's existence
/// is already known to the caller (e.g. a group member who isn't an admin) — see docs/04-API-DESIGN.md Section 1.7.
/// </summary>
public class ForbiddenException(string message) : Exception(message);
