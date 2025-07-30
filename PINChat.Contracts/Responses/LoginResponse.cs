namespace PINChat.Contracts.Responses;

public record LoginResponse
{
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required byte[] Avatar { get; set; } = [];
    public required string Message { get; set; }
}