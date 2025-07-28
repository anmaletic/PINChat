namespace PINChat.UI.ViewManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewManagerBase(this IServiceCollection services, Action<ViewManagerBuilder> configure)
    {
        services.AddSingleton<IViewManager>(sp =>
        {
            var viewManager = new ViewManager(sp);
            var builder = new ViewManagerBuilder(viewManager);
            configure(builder);
            return viewManager;
        });

        return services;
    }
}