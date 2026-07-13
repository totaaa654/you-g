using MediatR;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Auth.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<LoginCommand, AuthResultDto>
{
    // Deliberately the same message for "no such user" and "wrong password" — distinguishing them
    // lets an attacker enumerate registered emails.
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || user.IsDeleted || user.PasswordHash is null)
        {
            throw new AuthenticationFailedException(InvalidCredentialsMessage);
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException(InvalidCredentialsMessage);
        }

        var now = dateTimeProvider.UtcNow;
        var (accessToken, _) = tokenService.GenerateAccessToken(user);
        var rawRefreshToken = tokenService.GenerateRefreshToken();

        refreshTokenRepository.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(rawRefreshToken),
            ExpiresAt = now.AddDays(30),
            CreatedAt = now
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto(
            accessToken,
            rawRefreshToken,
            new UserSummaryDto(user.Id, user.Username, user.DisplayName, user.FriendCode));
    }
}
