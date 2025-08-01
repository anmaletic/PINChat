using FluentValidation;
using PINChat.UI.Core.Models;
using PINChat.UI.Core.Validators;

namespace PINChat.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services
            .AddTransient<MainViewModel>()
            .AddTransient<LoginViewModel>()
            .AddTransient<ChatViewModel>()
            .AddTransient<SignInViewModel>()
            .AddTransient<SignUpViewModel>()
            .AddTransient<IValidator<LoginUserModel>, LoginUserValidator>()
            .AddTransient<IValidator<RegistrationUserModel>, RegistrationUserValidator>();
        
        return services;
    }

    public static IServiceCollection AddViewManager(this IServiceCollection services)
    {
        services.AddViewManagerBase(builder =>
        {
            builder.RegisterView<LoginView, LoginViewModel>();
            builder.RegisterView<SignInView, SignInViewModel>();
            builder.RegisterView<SignUpView, SignUpViewModel>();
            builder.RegisterView<ChatView, ChatViewModel>();
        });

        return services;
    }
}