namespace YouG.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
}
