using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure.Push;

/// <summary>
/// Used when no Firebase service account credential is configured (CI, integration tests, local dev
/// without notifications set up). In-app Notification rows still get created — only the push leg is skipped.
/// </summary>
public class NoOpPushNotificationSender : IPushNotificationSender
{
    public Task<IReadOnlyList<string>> SendAsync(
        IReadOnlyList<string> tokens, string title, string body, IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<string>>([]);
}
