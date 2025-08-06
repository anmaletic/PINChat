namespace PINChat.Core.Application;

public interface IFileService
{
    Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> GetFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    string GetFileUrl(string bucketName, string objectName);
}