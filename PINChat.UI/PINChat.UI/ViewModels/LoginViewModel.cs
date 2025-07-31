using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class LoginViewModel : LoadableViewModelBase
{
    private readonly IValidator<LoginUserModel> _userValidator;
    private readonly IAuthApi _authApi;
    private readonly ILoggedInUserService _loggedInUserService;

    [ObservableProperty]
    private LoginUserModel _loginUser = new();
    
    [ObservableProperty]
    private string _message = "";

    public LoginViewModel(IValidator<LoginUserModel> userValidator, IAuthApi authApi, ILoggedInUserService loggedInUserService)
    {
        _userValidator = userValidator;
        _authApi = authApi;
        _loggedInUserService = loggedInUserService;
    }

    [RelayCommand]
    private async Task Login()
    {
        Message = "";
        
        var result = await _userValidator.ValidateAsync(LoginUser);

        if (!result.IsValid)
        {
            Message = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
            return;
        }
        
        IsLoading = true;

        var loginResult = await _authApi.Login(new() { UserName = LoginUser.Username, Password = LoginUser.Password});
        
        IsLoading = false;
        
        if (!loginResult.IsSuccessStatusCode)
        {
            Message = "Invalid username or password.";
            return;
        }
        
        var loggedInUserData = loginResult.Content!;
        
        _loggedInUserService.SetUser(loggedInUserData.ToModel(), loggedInUserData.UserId, loggedInUserData.Token);

        StrongReferenceMessenger.Default.Send(new ChangeViewMessage() { View = nameof(ChatViewModel) });
    }
    
    [RelayCommand]
    private void ChangeTheme()
    {    
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}