using Microsoft.Extensions.Hosting;

namespace Epoche.Shared.Services;

sealed class SingletonServiceWrapper<TService> : IHostedService where TService : class
{
    readonly IHostedService HostedService;

    public SingletonServiceWrapper(TService service)
    {
        HostedService = (service as IHostedService) ?? throw new InvalidOperationException($"{service?.GetType().Name} does not implement IHostedService");
    }
    public Task StartAsync(CancellationToken cancellationToken) => HostedService.StartAsync(cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken) => HostedService.StopAsync(cancellationToken);
}
