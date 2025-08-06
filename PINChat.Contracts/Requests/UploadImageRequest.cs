namespace PINChat.Contracts.Requests;

public record UploadImageRequest
{
    public IFormFile File { get; set; } = null!;
}