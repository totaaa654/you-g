using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;

namespace YouG.Application.Features.Profile.Commands.DeleteMyAccount;

/// <summary>
/// Soft-delete tombstone (docs/03-DATABASE.md Section 1): the row persists so every table
/// referencing this user (votes, attendance, memberships) keeps working, but PII is scrubbed.
/// Email/Username/FriendCode are NOT NULL + unique, so they're replaced with synthetic
/// unique placeholders derived from the user's own Id rather than nulled.
/// </summary>
public class DeleteMyAccountCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<DeleteMyAccountCommand>
{
    public async Task Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var now = dateTimeProvider.UtcNow;
        var idHex = user.Id.ToString("N");

        user.Email = $"deleted-{idHex[..12]}@deleted.youg.app";
        user.Username = $"deleted-{idHex[..12]}";
        user.FriendCode = $"DEL{idHex[..7]}"; // FriendCode is varchar(10): "DEL" + 7 hex chars
        user.DisplayName = "Deleted User";
        user.Bio = null;
        user.ProfilePictureUrl = null;
        user.PasswordHash = null;
        user.GoogleId = null;
        user.IsDeleted = true;
        user.DeletedAt = now;
        user.UpdatedAt = now;

        // Force logout everywhere — a deleted account shouldn't stay usable via an
        // already-issued refresh token.
        var activeTokens = await refreshTokenRepository.GetActiveByUserIdAsync(currentUser.UserId, cancellationToken);
        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
