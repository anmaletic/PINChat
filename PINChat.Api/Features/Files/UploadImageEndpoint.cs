using Microsoft.Extensions.Options;
using PINChat.Core.Application;
using PINChat.Core.Options;

namespace PINChat.Api.Features.Files;

public class UploadImageEndpoint : Endpoint<UploadImageRequest, UploadImageResponse>
{
    private readonly IFileService _fileService;
    private readonly MinioOptions _minioOptions;

    public UploadImageEndpoint(IFileService fileService, IOptions<MinioOptions> options)
    {
        _fileService = fileService;
        _minioOptions = options.Value;
    }

    public override void Configure()
    {
        Post(ApiEndpoints.Files.UploadImage);
        AuthSchemes(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme);
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Uploads an image file to MinIO.";
            s.Description = "Receives an image file, uploads it to the configured MinIO bucket, and returns the public URL.";
        });
    }

    public override async Task HandleAsync(UploadImageRequest req, CancellationToken ct)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (req.File == null || req.File.Length == 0)
        {
            AddError("No file uploaded or file is empty.", "NoFile");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var fileExtension = Path.GetExtension(req.File.FileName);
        var objectName = $"images/{currentUserId}/{Guid.NewGuid()}{fileExtension}";

        try
        {
            await using var stream = req.File.OpenReadStream();
            var imageUrl = await _fileService.UploadFileAsync(_minioOptions.BucketName, objectName, stream, req.File.ContentType, ct);

            await Send.ResponseAsync(new UploadImageResponse
            {
                ImagePath = imageUrl
            }, cancellation: ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file to MinIO: {ex.Message}");
            AddError($"File upload failed: {ex.Message}", "UploadFailed");
            await Send.ErrorsAsync(500, ct);
        }
    }
}