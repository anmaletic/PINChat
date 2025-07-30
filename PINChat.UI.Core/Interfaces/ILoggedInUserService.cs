using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Interfaces;

public interface ILoggedInUserService
{
    UserModel? User { get; }
    string? UserId { get; }
    string? UserToken { get; }
    bool IsLoggedIn { get; }

    void SetUser(UserModel user, string userId, string userToken);
    void ClearUser();
}