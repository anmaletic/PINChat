namespace PINChat.Contracts.Requests;

public record LoginRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
