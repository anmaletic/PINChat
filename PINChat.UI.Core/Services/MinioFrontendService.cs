using PINChat.Api.Sdk;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Models;
using Refit;

namespace PINChat.UI.Core.Services;

public class MinioFrontendService : IMinioFrontendService
{
    private readonly IChatApi _chatApi;
    private readonly UserModel _user;
    
    public MinioFrontendService(IChatApi chatApi, ILoggedInUserService loggedInUserService)
    {
        _chatApi = chatApi;
        _user = loggedInUserService.User!;
    }
    
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        try
        {
            // Create a StreamPart for Refit to send the file as multipart/form-data
            var filePart = new StreamPart(imageStream, fileName, contentType);

            // Call the backend API endpoint to upload the image
            var response = await _chatApi.UploadImage(filePart, $"Bearer {_user.Token}");

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                return response.Content.ImagePath;
            }
            else
            {
                var errorContent = response.Error?.Content ?? "Unknown error";
                Console.WriteLine(
                    $"[MinioFrontendService] Backend upload failed: {response.StatusCode} - {errorContent}");
                throw new Exception($"Image upload failed: {errorContent}");
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine($"[MinioFrontendService] API error during upload: {e.Message} - {e.Content}");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[MinioFrontendService] Unexpected error during upload: {e.Message}");
            throw;
        }
    }
}