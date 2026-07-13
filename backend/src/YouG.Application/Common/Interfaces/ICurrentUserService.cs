namespace YouG.Application.Common.Interfaces;

/// <summary>Who is making this request — resolved from the JWT by the API layer, consumed by handlers for authorization checks.</summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
}
