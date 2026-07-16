using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure.Email;

public class SendGridEmailSender(IOptions<EmailSettings> settings) : IEmailSender
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        var client = new SendGridClient(_settings.SendGridApiKey);
        var message = MailHelper.CreateSingleEmail(
            new EmailAddress(_settings.FromEmail, _settings.FromName), new EmailAddress(toEmail), subject,
            plainTextContent: null, htmlContent: htmlBody);

        var response = await client.SendEmailAsync(message, cancellationToken);
        if ((int)response.StatusCode >= 400)
        {
            var body = await response.Body.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"SendGrid rejected the email ({response.StatusCode}): {body}");
        }
    }
}
