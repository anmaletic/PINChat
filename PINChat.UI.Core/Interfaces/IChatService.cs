using PINChat.Contracts.Responses;

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

public interface IChatService
{
    event EventHandler<MessageReceivedEventArgs> MessageReceived;
    event EventHandler<MessageStatusUpdatedEventArgs> MessageStatusUpdated;
    
    event EventHandler<bool> ConnectionStatusChanged; // True for connected, False for disconnected

    bool IsConnected { get; }

    Task ConnectAsync();
    Task DisconnectAsync();

    Task SendMessageAsync(string recipientId, string content, string tempId);
    Task SendMessageReceivedStatusAsync(string messageId);
    Task SendMessageReadStatusAsync(string messageId);
}