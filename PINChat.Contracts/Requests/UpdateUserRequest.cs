namespace PINChat.Contracts.Requests;

public record UpdateUserRequest
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public byte[]? Avatar { get; set; } = [];
}