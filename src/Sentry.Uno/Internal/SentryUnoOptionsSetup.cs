using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sentry.Infrastructure;

namespace Sentry.Uno.Internal;

internal sealed class SentryUnoOptionsSetup : IConfigureOptions<SentryUnoOptions>
{
    private readonly IConfiguration _config;

    public SentryUnoOptionsSetup(IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config.GetSection("Sentry");
    }

    public void Configure(SentryUnoOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var bindable = new BindableSentryUnoOptions();
        _config.Bind(bindable);
        bindable.ApplyTo(options);

        // NOTE: Anything set here will overwrite options set by the user.
        //       For option defaults that can be changed, use the constructor in SentryUnoOptions instead.

        // We'll initialize the SDK in SentryUnoInitializer
        options.InitializeSdk = false;

        // Global Mode makes sense for client apps
        options.IsGlobalModeEnabled = true;

        // So debug logs are visible in both Rider and Visual Studio
        if (options is { Debug: true, DiagnosticLogger: null })
        {
            options.DiagnosticLogger = new ConsoleAndTraceDiagnosticLogger(options.DiagnosticLevel);
        }

        // We'll use an event processor to set things like SDK name
        options.AddEventProcessor(new SentryUnoEventProcessor(options));

        if (options.AttachScreenshot)
        {
            options.AddEventProcessor(new SentryUnoScreenshotProcessor(options));
        }
    }
}
