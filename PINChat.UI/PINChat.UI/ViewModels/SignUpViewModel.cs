using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;
using PINChat.Api.Sdk;
using PINChat.UI.Core.Messages;
using PINChat.UI.Core.Models;

namespace PINChat.UI.ViewModels;

public partial class SignUpViewModel : LoginViewModelBase
{
    private readonly IValidator<RegistrationUserModel> _registrationUserValidator;
    private readonly IAuthApi _authApi;
    
    [ObservableProperty]
    private RegistrationUserModel _user = new();
    
    public SignUpViewModel() : this(null!, null!)
    {
    }
    
    public SignUpViewModel(IValidator<RegistrationUserModel> registrationUserValidator,
        IAuthApi authApi)
    {
        _registrationUserValidator = registrationUserValidator;
        _authApi = authApi;
    }

    [RelayCommand]
    private async Task SignUp()
    {
        Message = "";
        
        var result = await _registrationUserValidator.ValidateAsync(User);

        if (!result.IsValid)
        {
            Message = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
            return;
        }
        
        IsLoading = true;

        var registrationResult = await _authApi.Register(new()
        {
            UserName = User.Username,
            Password = User.Password,
            Email = User.Email,
            FirstName = string.Empty,
            LastName = string.Empty
        });
        
        IsLoading = false;
        
        if (!registrationResult.IsSuccessStatusCode)
        {
            Message = "Registration failed. Please try again.";
            return;
        }
        
        Message = "Successfully registered.";
        await Task.Delay(800);
        
        ChangeRegistrationState();
    }
    
    
    [RelayCommand]
    private void ChangeRegistrationState()
    {
        StrongReferenceMessenger.Default.Send(new ChangeLoginViewMessage(nameof(SignInViewModel)));
    }
}