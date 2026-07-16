using Microsoft.Extensions.Logging;
using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure.Email;

/// <summary>
/// Used when no SendGrid API key is configured (CI, integration tests, local dev before an email
/// provider is set up). Logs the would-be email — including the reset link — so the flow is still
/// testable end-to-end without a real provider.
/// </summary>
public class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Email sending is not configured — would have sent to {ToEmail}, subject {Subject}:\n{Body}",
            toEmail, subject, htmlBody);
        return Task.CompletedTask;
    }
}
