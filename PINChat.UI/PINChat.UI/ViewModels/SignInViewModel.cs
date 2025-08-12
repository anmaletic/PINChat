using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Interfaces;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class SignInViewModel : LoginViewModelBase
{
    private readonly IValidator<LoginUserModel> _loginUserValidator;
    private readonly IAuthApi _authApi;
    private readonly ILoggedInUserService _loggedInUserService;

    [ObservableProperty]
    private LoginUserModel _user = new();

    public SignInViewModel() : this(null!, null!, null!)
    {
    }

    public SignInViewModel(IValidator<LoginUserModel> loginUserValidator,
        IAuthApi authApi, ILoggedInUserService loggedInUserService)
    {
        _loginUserValidator = loginUserValidator;
        _authApi = authApi;
        _loggedInUserService = loggedInUserService;
    }

    [RelayCommand]
    private async Task SignIn()
    {
        Message = "";

        var result = await _loginUserValidator.ValidateAsync(User);

        if (!result.IsValid)
        {
            Message = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
            return;
        }

        IsLoading = true;

        var loginResult = await _authApi.Login(new() { UserName = User.Username, Password = User.Password });

        IsLoading = false;

        if (!loginResult.IsSuccessStatusCode)
        {
            Message = "Invalid username or password.";
            return;
        }

        Console.WriteLine("Login successful");
        Console.WriteLine($"Login response: {loginResult?.ToString() ?? "NULL"}");
        Console.WriteLine("Data: " + loginResult.Content);
        
        var loggedInUserData = loginResult.Content!;

        _loggedInUserService.SetUser(loggedInUserData.ToModel(), loggedInUserData.UserId, loggedInUserData.Token!);

        StrongReferenceMessenger.Default.Send(new IsViewDeactivatedMessage(true));
        StrongReferenceMessenger.Default.Send(new ChangeViewMessage(nameof(ChatViewModel)));
    }

    [RelayCommand]
    private void ChangeRegistrationState()
    {
        StrongReferenceMessenger.Default.Send(new ChangeLoginViewMessage(nameof(SignUpViewModel)));
    }
}