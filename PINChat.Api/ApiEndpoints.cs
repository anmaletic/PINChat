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
}