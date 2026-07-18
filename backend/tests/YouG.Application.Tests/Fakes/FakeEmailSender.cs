using YouG.Application.Common.Interfaces;

namespace YouG.Application.Tests.Fakes;

public class FakeEmailSender : IEmailSender
{
    public record SentEmail(string ToEmail, string Subject, string HtmlBody);

    public List<SentEmail> Sent { get; } = [];

    public Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        Sent.Add(new SentEmail(toEmail, subject, htmlBody));
        return Task.CompletedTask;
    }
}
