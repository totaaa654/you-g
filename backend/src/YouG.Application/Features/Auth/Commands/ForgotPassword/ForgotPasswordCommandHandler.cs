using MediatR;
using Microsoft.Extensions.Options;
using YouG.Application.Common;
using YouG.Application.Common.Interfaces;
using YouG.Domain.Entities;

namespace YouG.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenRepository resetTokenRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    IEmailSender emailSender,
    IOptions<ClientUrlSettings> clientUrlSettings,
    IDateTimeProvider dateTimeProvider) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Always succeeds from the caller's perspective, whether or not the email is registered —
        // distinguishing the two lets an attacker enumerate accounts (same rationale as Login's
        // generic "Invalid email or password"). A Google-only account (no PasswordHash) has no
        // password to reset, so it's treated the same as "not found" here.
        if (user is null || user.IsDeleted || user.PasswordHash is null)
        {
            return;
        }

        var now = dateTimeProvider.UtcNow;

        // Only one active code per user at a time — requesting a new one retires any earlier,
        // unused code so there's no ambiguity about which code is current.
        var pending = await resetTokenRepository.GetPendingForUserAsync(user.Id, cancellationToken);
        foreach (var old in pending)
        {
            old.UsedAt = now;
        }

        var code = tokenService.GenerateOtpCode();

        resetTokenRepository.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(code),
            ExpiresAt = now.AddMinutes(10),
            CreatedAt = now
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var resetUrl =
            $"{clientUrlSettings.Value.ResetPasswordUrlBase}?email={Uri.EscapeDataString(user.Email)}&code={code}";
        var body = $"""
            <p>Hi {user.DisplayName},</p>
            <p>Someone requested a password reset for your You G? account. Your code is:</p>
            <p style="font-family: monospace; font-size: 28px; letter-spacing: 6px; background:#f0f0f0; padding:12px 16px; text-align:center;">{code}</p>
            <p>It expires in 10 minutes. Enter it in the app, or use the link below to have it filled
            in automatically:</p>
            <p><a href="{resetUrl}">Reset your password</a></p>
            <p>If you didn't request this, you can safely ignore this email.</p>
            """;

        await emailSender.SendAsync(user.Email, "Your You G? password reset code", body, cancellationToken);
    }
}
