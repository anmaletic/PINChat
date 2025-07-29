namespace PINChat.UI.Core.Models;

public class ChatMessageModel
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Sender { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsSent { get; set; } = false;
    public bool IsReceived { get; set; } = false;
    public bool IsRead { get; set; } = false;
    public bool IsOrigin { get; set; }
}