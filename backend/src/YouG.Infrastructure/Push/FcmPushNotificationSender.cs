using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure.Push;

public class FcmPushNotificationSender(ILogger<FcmPushNotificationSender> logger) : IPushNotificationSender
{
    public async Task<IReadOnlyList<string>> SendAsync(
        IReadOnlyList<string> tokens, string title, string body, IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken)
    {
        if (tokens.Count == 0)
        {
            return [];
        }

        var message = new MulticastMessage
        {
            Tokens = tokens as List<string> ?? tokens.ToList(),
            Notification = new FirebaseAdmin.Messaging.Notification { Title = title, Body = body },
            Data = data as Dictionary<string, string> ?? data.ToDictionary(kv => kv.Key, kv => kv.Value)
        };

        BatchResponse response;
        try
        {
            response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Push delivery is best-effort — the in-app Notification row already exists regardless.
            logger.LogWarning(ex, "Push notification send failed for {TokenCount} device(s).", tokens.Count);
            return [];
        }

        var deadTokens = new List<string>();
        for (var i = 0; i < response.Responses.Count; i++)
        {
            var result = response.Responses[i];
            if (!result.IsSuccess &&
                result.Exception?.MessagingErrorCode is MessagingErrorCode.Unregistered or MessagingErrorCode.InvalidArgument)
            {
                deadTokens.Add(tokens[i]);
            }
        }

        return deadTokens;
    }
}
