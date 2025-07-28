namespace PINChat.UI.ViewManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewManager(this IServiceCollection services)
    {
        services.AddSingleton<IViewManager, ViewManagerBase>();
        return services;
    }

    public static IServiceCollection RegisterViewWithViewManager<TView, TViewModel>(this IServiceCollection services,
        string? viewName = null)
        where TView : class, new()
        where TViewModel : notnull
    {
        services.PostConfigure<IViewManager>(viewManager => 
        {
            viewManager.RegisterView<TView, TViewModel>(viewName);
        });

        return services;
    }
}