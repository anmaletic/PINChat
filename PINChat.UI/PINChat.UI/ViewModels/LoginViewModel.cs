using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class LoginViewModel : LoadableViewModelBase
{
    private readonly IValidator<LoginUserModel> _userValidator;

    [ObservableProperty]
    private LoginUserModel _loginUser = new();
    
    [ObservableProperty]
    private string _message = "";

    public LoginViewModel(IValidator<LoginUserModel> userValidator)
    {
        _userValidator = userValidator;
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

        // simulate a login delay
        await Task.Delay(2000);
        
        IsLoading = false;

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