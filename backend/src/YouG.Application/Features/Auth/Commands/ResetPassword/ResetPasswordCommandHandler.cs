using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository resetTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<ResetPasswordCommand>
{
    // 6-digit codes have far less entropy than the old high-entropy blob, so a cap on attempts
    // matters here in a way it didn't before — otherwise the code space (1,000,000 values) is
    // brute-forceable within the 10-minute expiry window.
    private const int MaxAttempts = 5;

    // Deliberately generic — same message whether the email is unknown, the code is wrong,
    // expired, or attempts are exhausted (same enumeration rationale as Login).
    private const string InvalidCodeMessage = "This code is invalid or has expired.";

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || user.IsDeleted)
        {
            throw new AuthenticationFailedException(InvalidCodeMessage);
        }

        var now = dateTimeProvider.UtcNow;
        var pending = await resetTokenRepository.GetPendingForUserAsync(user.Id, cancellationToken);
        var resetToken = pending.OrderByDescending(t => t.CreatedAt).FirstOrDefault();

        if (resetToken is null || resetToken.ExpiresAt <= now)
        {
            throw new AuthenticationFailedException(InvalidCodeMessage);
        }

        if (resetToken.FailedAttempts >= MaxAttempts)
        {
            resetToken.UsedAt = now;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new AuthenticationFailedException(InvalidCodeMessage);
        }

        if (resetToken.TokenHash != tokenService.HashToken(request.Code))
        {
            resetToken.FailedAttempts++;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new AuthenticationFailedException(InvalidCodeMessage);
        }

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        resetToken.UsedAt = now;

        // Resetting the password invalidates every existing session — if someone else had gained
        // access to the account, this locks them out immediately.
        var activeSessions = await refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
        foreach (var session in activeSessions)
        {
            session.RevokedAt = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
