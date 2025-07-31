namespace PINChat.Contracts.Responses;

public record MessageResponse
{
    public required string Id { get; set; }
    public required DateTime Timestamp { get; set; }
    public required string SenderId { get; set; }
    public required string RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
    
    public bool IsSent { get; set; } = false;
    public bool IsReceived { get; set; } = false;
    public bool IsRead { get; set; } = false;
    
    public string TempId { get; set; } = string.Empty;
    
}