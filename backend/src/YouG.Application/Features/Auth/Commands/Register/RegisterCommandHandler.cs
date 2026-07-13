using MediatR;
using YouG.Application.Common;
using YouG.Application.Common.Exceptions;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Auth.Dtos;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("An account with this email already exists.");
        }

        if (await userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
        {
            throw new ConflictException("This username is already taken.");
        }

        var friendCode = await GenerateUniqueFriendCodeAsync(cancellationToken);
        var now = dateTimeProvider.UtcNow;

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            Username = request.Username,
            FriendCode = friendCode,
            DisplayName = request.DisplayName,
            TimeZoneId = request.TimeZoneId,
            CreatedAt = now,
            UpdatedAt = now
        };

        userRepository.Add(user);

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

    private async Task<string> GenerateUniqueFriendCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = FriendCodeGenerator.Generate();
            if (!await userRepository.ExistsByFriendCodeAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Failed to generate a unique friend code after 5 attempts.");
    }
}
