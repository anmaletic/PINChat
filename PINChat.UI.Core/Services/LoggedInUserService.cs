using CommunityToolkit.Mvvm.ComponentModel;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Services;

public partial class LoggedInUserService : ObservableObject, ILoggedInUserService
{
    [ObservableProperty]
    private UserModel? _user;

    [ObservableProperty]
    private string? _userId;

    [ObservableProperty]
    private string? _userToken;

    public bool IsLoggedIn => User != null && !string.IsNullOrEmpty(UserToken);

    public void SetUser(UserModel user, string userId, string userToken)
    {
        User = user;
        UserId = userId;
        UserToken = userToken;
        OnPropertyChanged(nameof(IsLoggedIn)); // Notify that IsLoggedIn status changed
    }

    public void ClearUser()
    {
        User = null;
        UserId = null;
        UserToken = null;
        OnPropertyChanged(nameof(IsLoggedIn)); // Notify that IsLoggedIn status changed
    }
}