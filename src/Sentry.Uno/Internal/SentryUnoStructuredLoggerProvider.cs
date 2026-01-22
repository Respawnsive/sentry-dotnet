using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry.Extensions.Logging;
using Sentry.Infrastructure;

namespace Sentry.Uno.Internal;

/// <summary>
/// Sentry Logger Provider for <see cref="SentryLog"/>.
/// </summary>
[ProviderAlias("Sentry")]
internal sealed class SentryUnoStructuredLoggerProvider : SentryStructuredLoggerProvider
{
    public SentryUnoStructuredLoggerProvider(IOptions<SentryUnoOptions> options, IHub hub)
        : this(options.Value, hub, SystemClock.Clock, CreateSdkVersion())
    {
    }

    internal SentryUnoStructuredLoggerProvider(SentryUnoOptions options, IHub hub, ISystemClock clock, SdkVersion sdk)
        : base(options, hub, clock, sdk)
    {
    }

    private static SdkVersion CreateSdkVersion()
    {
        return new SdkVersion
        {
            Name = Constants.SdkName,
            Version = Constants.SdkVersion,
        };
    }
}
