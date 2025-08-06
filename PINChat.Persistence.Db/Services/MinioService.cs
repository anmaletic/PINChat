using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using PINChat.Core.Application;
using PINChat.Core.Options;

namespace PINChat.Persistence.Db.Services;

public class MinioService : IFileService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _minioOptions;

    public MinioService(IOptions<MinioOptions> minioOptions)
    {
        _minioOptions = minioOptions.Value;
        
        _minioClient = new MinioClient()
            .WithEndpoint(_minioOptions.Endpoint)
            .WithCredentials(_minioOptions.AccessKey, _minioOptions.SecretKey)
            .WithSSL()
            .Build();
    }
    
    public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(args, cancellationToken).ConfigureAwait(false);
            return GetFileUrl(bucketName, objectName);
        }
        catch (MinioException e)
        {
            Console.WriteLine($"[MinioService] Error uploading file: {e.Message}");
            throw;
        }
    }
    

    public async Task<Stream> GetFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream((stream) => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(args, cancellationToken).ConfigureAwait(false);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (MinioException e)
        {
            Console.WriteLine($"[MinioService] Error getting file: {e.Message}");
            throw;
        }
    }

    public string GetFileUrl(string bucketName, string objectName)
    {
        var protocol = _minioOptions.UseSsl ? "https" : "http";
        return $"{protocol}://{_minioOptions.Endpoint}/{bucketName}/{objectName}";
    }
}