using FluentValidation;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Validators;

public class LoginUserValidator : AbstractValidator<LoginUserModel>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}