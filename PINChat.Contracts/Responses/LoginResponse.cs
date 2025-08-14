namespace PINChat.Contracts.Responses;

public record LoginResponse
{
    public string? Token { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required byte[] Avatar { get; set; } = [];
    public string? AvatarPath { get; set; }
    public IEnumerable<LoginResponse> Contacts { get; set; } = [];
    public IEnumerable<LoginResponse> AddedByOthers { get; set; } = [];
    public required string Message { get; set; }
}