namespace PINChat.Contracts.Responses;

public record UploadImageResponse
{
    public string ImagePath { get; set; } = null!;
}