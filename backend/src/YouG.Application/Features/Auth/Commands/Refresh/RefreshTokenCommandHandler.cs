using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.Application.Features.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private const string InvalidTokenMessage = "Invalid or expired refresh token.";

    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashToken(request.RefreshToken);
        var existingToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        var now = dateTimeProvider.UtcNow;

        if (existingToken is null || existingToken.RevokedAt is not null || existingToken.ExpiresAt <= now)
        {
            throw new AuthenticationFailedException(InvalidTokenMessage);
        }

        var user = await userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user is null || user.IsDeleted)
        {
            throw new AuthenticationFailedException(InvalidTokenMessage);
        }

        var (accessToken, _) = tokenService.GenerateAccessToken(user);
        var newRawRefreshToken = tokenService.GenerateRefreshToken();

        var newToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(newRawRefreshToken),
            ExpiresAt = now.AddDays(30),
            CreatedAt = now
        };

        refreshTokenRepository.Add(newToken);

        // Rotate: the presented token is immediately invalidated so it can't be replayed.
        existingToken.RevokedAt = now;
        existingToken.ReplacedByTokenId = newToken.Id;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto(
            accessToken,
            newRawRefreshToken,
            new UserSummaryDto(user.Id, user.Username, user.DisplayName, user.FriendCode));
    }
}
