namespace PINChat.Contracts.Responses;


public record UserResponse
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarPath { get; set; }
    public byte[]? Avatar { get; set; }
}