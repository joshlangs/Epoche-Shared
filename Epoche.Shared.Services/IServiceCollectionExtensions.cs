using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Epoche.Shared.Services;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonHostedService<TServiceInterface, TService>(this IServiceCollection services)
        where TServiceInterface : class
        where TService : class, IHostedService, TServiceInterface
        => services
            .AddSingleton<TServiceInterface, TService>()
            .AddHostedService<SingletonServiceWrapper<TServiceInterface>>();
}
