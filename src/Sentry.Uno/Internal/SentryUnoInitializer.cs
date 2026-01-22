using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sentry.Uno.Internal;

internal sealed class SentryUnoInitializer : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly Disposer _disposer;

    public SentryUnoInitializer(IServiceProvider services, Disposer disposer)
    {
        _services = services;
        _disposer = disposer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var options = _services.GetRequiredService<IOptions<SentryUnoOptions>>().Value;
        var disposable = SentrySdk.Init(options);
        _disposer.Register(disposable);

#if !PLATFORM_NEUTRAL
        var lifecycleBinder = _services.GetService<IUnoLifecycleBinder>();
        lifecycleBinder?.Bind();
#endif

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _disposer.Dispose();
        return Task.CompletedTask;
    }
}
