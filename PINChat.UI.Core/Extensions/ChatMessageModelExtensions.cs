using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Extensions;

public static class ChatMessageModelExtensions
{
    public static void UpdateOrigin(this ChatMessageModel message, string userId)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        message.IsOrigin = message.SenderId == userId;
    }
}