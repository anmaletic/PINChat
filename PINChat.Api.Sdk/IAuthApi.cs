namespace PINChat.Api.Sdk;

public interface IAuthApi
{
    [Post(ApiEndpoints.Auth.Register)]
    Task<ApiResponse<RegisterResponse>> Register([Body] RegisterRequest request);
    
    [Post(ApiEndpoints.Auth.Login)]
    Task<ApiResponse<LoginResponse>> Login([Body] LoginRequest request);
    
}