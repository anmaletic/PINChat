using System.Collections.ObjectModel;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Extensions;

public static class CollectionExtensions
{
    public static void UpdateMessageOrigin(this IEnumerable<ChatMessageModel> messages, string userId)
    {
        if (messages == null) throw new ArgumentNullException(nameof(messages));
        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        foreach (var message in messages)
        {
            message.UpdateOrigin(userId);
        }
    }
}