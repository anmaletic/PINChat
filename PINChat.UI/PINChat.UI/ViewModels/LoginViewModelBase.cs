using CommunityToolkit.Mvvm.Input;

namespace PINChat.UI.ViewModels;

public partial class LoginViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isRegistered;

    [ObservableProperty]
    private string _message = string.Empty;
}