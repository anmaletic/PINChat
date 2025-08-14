namespace PINChat.Contracts.Requests;

public record UserContactRequest
{
    public string UserId { get; init; }
    public required string ContactId { get; init; }
}