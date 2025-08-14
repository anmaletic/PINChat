using FluentValidation;
using PINChat.UI.Core.Models;
using PINChat.UI.Core.Validators;
using PINChat.UI.Views.Mobile;

namespace PINChat.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services
            .AddTransient<MainViewModel>()
            .AddTransient<ErrorDialogViewModel>()
            .AddTransient<LoginViewModel>()
            .AddTransient<ChatViewModel>()
            .AddTransient<ProfileViewModel>()
            .AddTransient<MessagingViewModel>()
            .AddTransient<ContactsViewModel>()
            .AddTransient<SignInViewModel>()
            .AddTransient<SignUpViewModel>();
        
        return services;
    }
    
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services
            .AddTransient<IValidator<LoginUserModel>, LoginUserValidator>()
            .AddTransient<IValidator<RegistrationUserModel>, RegistrationUserValidator>();
        
        return services;
    }

    public static IServiceCollection AddViewManager(this IServiceCollection services)
    {
        services.AddViewManagerBase(builder =>
        {
            builder.RegisterView<ErrorDialogView, ErrorDialogViewModel>();
            builder.RegisterView<LoginView, LoginViewModel>();
            
            builder.RegisterView<SignInView, SignInViewModel>("SignInView");
            builder.RegisterView<SignUpView, SignUpViewModel>("SignUpView");
            
            builder.RegisterView<SignInMobileView, SignInViewModel>("SignInMobileView");
            builder.RegisterView<SignUpMobileView, SignUpViewModel>("SignUpMobileView");
            
            builder.RegisterView<ChatView, ChatViewModel>("ChatView");
            
            builder.RegisterView<ProfileView, ProfileViewModel>("ProfileView");
            builder.RegisterView<ProfileMobileView, ProfileViewModel>("ProfileMobileView");
            
            builder.RegisterView<MessagingView, MessagingViewModel>("MessagingView");
            builder.RegisterView<MessagingMobileView, MessagingViewModel>("MessagingMobileView");

            builder.RegisterView<ContactsView, ContactsViewModel>("ContactsView");
        });

        return services;
    }
}