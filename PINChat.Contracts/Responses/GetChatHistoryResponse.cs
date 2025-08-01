namespace PINChat.Contracts.Responses;

public record GetChatHistoryResponse
{
    public IEnumerable<MessageResponse> Messages { get; set; } = [];
}