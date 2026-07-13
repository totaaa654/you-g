using Microsoft.AspNetCore.Identity;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Auth;

/// <summary>
/// Wraps ASP.NET Core Identity's PasswordHasher (PBKDF2, adaptive) rather than pulling in a
/// third-party hashing library — it satisfies NFR-4 (modern adaptive hash) with zero extra
/// dependencies (docs/01-PRD.md).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _identityHasher = new();

    public string Hash(string password) => _identityHasher.HashPassword(default!, password);

    public bool Verify(string password, string hash) =>
        _identityHasher.VerifyHashedPassword(default!, hash, password) != PasswordVerificationResult.Failed;
}
