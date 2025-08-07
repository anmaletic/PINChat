namespace PINChat.UI.Core.Helpers;

public static class PlatformHelper
{
    public static bool IsMobile()
    {
        return OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
    }
    
    public static string SignInView => IsMobile() ? "SignInMobileView" : "SignInView";
    public static string SignUpView => IsMobile() ? "SignUpMobileView" : "SignUpView";
    public static string ChatView => IsMobile() ? "ChatMobileView" : "ChatView";
}