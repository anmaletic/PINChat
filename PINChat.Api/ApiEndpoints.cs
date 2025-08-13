namespace PINChat.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Auth
    {
        private const string Base = $"{ApiBase}/auth";
        
        public const string Register = $"{Base}/register";
        public const string Login = $"{Base}/login";
    }
    
    public static class Messages
    {
        private const string Base = $"{ApiBase}/messages";
        
        public const string GetChatHistory = $"{Base}/history/{{ContactId}}";
    }
    
    public static class Files
    {
        private const string Base = $"{ApiBase}/files";
        
        public const string UploadImage = $"{Base}/upload-image";
    }
    
    public static class Users
    {
        private const string Base = $"{ApiBase}/users";
        
        public const string GetAll = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
    }
}