namespace PINChat.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services
            .AddTransient<MainViewModel>()
            .AddTransient<LoginViewModel>()
            .AddTransient<ChatViewModel>();
        
        return services;
    }

    public static IServiceCollection AddViewManager(this IServiceCollection services)
    {
        services.AddViewManagerBase(builder =>
        {
            builder.RegisterView<LoginView, LoginViewModel>();
            builder.RegisterView<ChatView, ChatViewModel>();
        });

        return services;
    }
}