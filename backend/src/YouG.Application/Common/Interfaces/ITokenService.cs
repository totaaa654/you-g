using YouG.Domain.Entities;

namespace YouG.Application.Common.Interfaces;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user);

    /// <summary>Raw, high-entropy refresh token — never stored as-is, only its hash (see HashToken).</summary>
    string GenerateRefreshToken();

    /// <summary>Deterministic hash used both to store and to look up a refresh token by its raw value.</summary>
    string HashToken(string rawToken);

    /// <summary>Short numeric code (e.g. for password-reset OTPs) — easy to read and type/paste
    /// by hand, unlike <see cref="GenerateRefreshToken"/>'s high-entropy blob.</summary>
    string GenerateOtpCode();
}
