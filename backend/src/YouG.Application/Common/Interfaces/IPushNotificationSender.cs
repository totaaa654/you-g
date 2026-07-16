namespace YouG.Application.Common.Interfaces;

public interface IPushNotificationSender
{
    /// <summary>
    /// Sends the same push to every token. Returns the subset of <paramref name="tokens"/> that Firebase
    /// reported as invalid/unregistered, so the caller can prune them from storage.
    /// </summary>
    Task<IReadOnlyList<string>> SendAsync(
        IReadOnlyList<string> tokens, string title, string body, IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken);
}
