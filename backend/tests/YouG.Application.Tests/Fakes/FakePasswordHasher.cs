using YouG.Application.Common.Interfaces;

namespace YouG.Application.Tests.Fakes;

/// <summary>Deliberately not a real hash — deterministic and fast, which is all a unit test needs.</summary>
public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string hash) => hash == $"hashed:{password}";
}
