using YouG.Application.Common.Interfaces;

namespace YouG.Application.Tests.Fakes;

public class FakeCurrentUserService(Guid userId) : ICurrentUserService
{
    public Guid UserId { get; } = userId;
}
