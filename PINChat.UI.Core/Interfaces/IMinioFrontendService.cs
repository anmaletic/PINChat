namespace PINChat.UI.Core.Interfaces;

public interface IMinioFrontendService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
}