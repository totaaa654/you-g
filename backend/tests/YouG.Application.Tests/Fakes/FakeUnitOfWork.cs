using YouG.Application.Common.Interfaces;

namespace YouG.Application.Tests.Fakes;

/// <summary>No-op — the fake repositories already mutate their in-memory lists directly on Add().</summary>
public class FakeUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
