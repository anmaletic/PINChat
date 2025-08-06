using PINChat.Contracts.Responses;
using PINChat.Core.Domain.Enums;

namespace PINChat.UI.Core.Interfaces;

public class MessageReceivedEventArgs(MessageResponse message) : EventArgs
{
    public MessageResponse Message { get; } = message;
}

public class MessageStatusUpdatedEventArgs(string messageId, string status) : EventArgs
{
    public string MessageId { get; } = messageId;
    public string Status { get; } = status;
}

public class TypingStatusEventArgs(string userId, bool isTyping) : EventArgs
{
    public string UserId { get; } = userId;
    public bool IsTyping { get; } = isTyping;
}

public interface IChatService
{
    event EventHandler<MessageReceivedEventArgs> MessageReceived;
    event EventHandler<MessageStatusUpdatedEventArgs> MessageStatusUpdated;
    event EventHandler<bool> ConnectionStatusChanged;
    event EventHandler<TypingStatusEventArgs> TypingStatusReceived;

    bool IsConnected { get; }

    Task ConnectAsync();
    Task DisconnectAsync();

    Task SendMessageAsync(string recipientId, string? content, string tempId, MessageType messageType, string? imagePath);
    Task SendMessageReceivedStatusAsync(string messageId);
    Task SendMessageReadStatusAsync(string messageId);
    Task SendTypingStatusAsync(string recipientId, bool isTyping);
}