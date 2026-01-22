using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry.Extensibility;
using Sentry.Extensions.Logging.Extensions.DependencyInjection;
using Sentry.Uno;
using Sentry.Uno.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Sentry extensions for <see cref="IHostBuilder"/> in Uno Platform.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SentryUnoHostBuilderExtensions
{
    /// <summary>
    /// Uses Sentry integration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The <paramref name="builder"/>.</returns>
    public static IHostBuilder UseSentryUno(this IHostBuilder builder)
        => UseSentryUno(builder, (Action<SentryUnoOptions>?)null);

    /// <summary>
    /// Uses Sentry integration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="dsn">The DSN.</param>
    /// <returns>The <paramref name="builder"/>.</returns>
    public static IHostBuilder UseSentryUno(this IHostBuilder builder, string dsn)
        => builder.UseSentryUno(o => o.Dsn = dsn);

    /// <summary>
    /// Uses Sentry integration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureOptions">An action to configure the options.</param>
    /// <returns>The <paramref name="builder"/>.</returns>
    public static IHostBuilder UseSentryUno(this IHostBuilder builder,
        Action<SentryUnoOptions>? configureOptions)
    {
        builder.ConfigureServices((_, services) =>
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            services.AddLogging();
            services.AddSingleton<ILoggerProvider, SentryUnoLoggerProvider>();
            services.AddSingleton<ILoggerProvider, SentryUnoStructuredLoggerProvider>();
            services.AddSingleton<IConfigureOptions<SentryUnoOptions>, SentryUnoOptionsSetup>();
            services.AddSingleton<Disposer>();
            services.AddSingleton<IHostedService, SentryUnoInitializer>();

#if !PLATFORM_NEUTRAL
            services.TryAddSingleton<IUnoLifecycleBinder, UnoLifecycleBinder>();
            services.TryAddSingleton<IUnoEventsBinder, UnoEventsBinder>();
            services.TryAddSingleton<UnoWindowTracker>();
#endif

            services.AddSentry<SentryUnoOptions>();
        });

        return builder;
    }
}
