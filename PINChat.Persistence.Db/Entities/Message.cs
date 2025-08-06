namespace PINChat.Persistence.Db.Entities;

public class Message
{
    public required string Id { get; set; }
    public required DateTime Timestamp { get; set; }
    public required string SenderId { get; set; }
    public required string RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
    
    public MessageType MessageType { get; set; } = MessageType.Text;
    public string? ImagePath { get; set; } 
    
    public bool IsSent { get; set; } = false;
    public bool IsReceived { get; set; } = false;
    public bool IsRead { get; set; } = false;
    
    public ApplicationUser Sender { get; set; }
    public ApplicationUser Recipient { get; set; }
}