namespace PINChat.Contracts.Responses;

public record RegisterResponse
{
    public required string UserId { get; set; }
    public required string Message { get; set; }
}