namespace PINChat.Contracts.Responses;

public record GetAllUsersResponse
{
    public List<UserResponse> Users { get; set; } = new();
}