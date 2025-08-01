namespace PINChat.Api.Sdk;

public interface IChatApi
{
    [Get(ApiEndpoints.Messages.GetChatHistory)]
    Task<ApiResponse<GetChatHistoryResponse>> GetChatHistory(string contactId, [Header("Authorization")] string authorization);
}