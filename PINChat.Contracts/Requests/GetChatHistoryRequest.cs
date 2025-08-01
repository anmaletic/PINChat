namespace PINChat.Contracts.Requests;

public record GetChatHistoryRequest
{
    public required string ContactId { get; set; }
}