using FluentValidation;
using PINChat.UI.Core.Models;

namespace PINChat.UI.Core.Validators;

public class RegistrationUserValidator : AbstractValidator<RegistrationUserModel>
{
    public RegistrationUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(20)
            .WithMessage("Username must not exceed 20 characters.");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(50)
            .WithMessage("Password must not exceed 50 characters.");
        
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
    
}