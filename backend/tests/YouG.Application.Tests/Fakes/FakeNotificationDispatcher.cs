using YouG.Application.Common.Interfaces;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Fakes;

public class FakeNotificationDispatcher : INotificationDispatcher
{
    public record DispatchedNotification(Guid UserId, NotificationType Type, string Title, string Body);

    public List<DispatchedNotification> Dispatched { get; } = [];

    public Task DispatchAsync(
        Guid userId, NotificationType type, string title, string body,
        IReadOnlyDictionary<string, string> payload, CancellationToken cancellationToken)
    {
        Dispatched.Add(new DispatchedNotification(userId, type, title, body));
        return Task.CompletedTask;
    }
}
