namespace PINChat.Api.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiSdk(this IServiceCollection services)
    {
        var apiUrl = new Uri("https://localhost:7233");

        services
            .AddHttpClient();

        services
            .AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(client => { client.BaseAddress = apiUrl; });

        return services;
    }
    
}