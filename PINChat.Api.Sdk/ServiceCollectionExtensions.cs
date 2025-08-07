namespace PINChat.Api.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiSdk(this IServiceCollection services)
    {
        var apiUrl = new Uri("https://pinchat-v2-api.anmal.dev");

        services
            .AddHttpClient();

        services
            .AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(client => { client.BaseAddress = apiUrl; });
        
        services
            .AddRefitClient<IChatApi>()
            .ConfigureHttpClient(client => { client.BaseAddress = apiUrl; });

        return services;
    }
    
}