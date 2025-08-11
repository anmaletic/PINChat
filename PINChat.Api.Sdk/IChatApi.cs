namespace PINChat.Api.Sdk;

public interface IChatApi
{
    [Get(ApiEndpoints.Messages.GetChatHistory)]
    Task<ApiResponse<GetChatHistoryResponse>> GetChatHistory(string contactId,
        [Header("Authorization")] string authorization,
        CancellationToken cancellationToken);
    
    [Post(ApiEndpoints.Files.UploadImage)]
    [Multipart]
    Task<ApiResponse<UploadImageResponse>> UploadImage([AliasAs("File")] StreamPart file,
        [Header("Authorization")] string authorization);

}