using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sentry.Extensions.Logging;
using Sentry.Infrastructure;

namespace Sentry.Uno.Internal;

/// <summary>
/// Sentry Logger Provider for <see cref="Breadcrumb"/> and <see cref="SentryEvent"/>.
/// </summary>
[ProviderAlias("Sentry")]
internal sealed class SentryUnoLoggerProvider : SentryLoggerProvider
{
    public SentryUnoLoggerProvider(IOptions<SentryUnoOptions> options, IHub hub)
        : base(options, hub)
    {
    }

    internal SentryUnoLoggerProvider(SentryUnoOptions options, IHub hub, ISystemClock clock)
        : base(hub, clock, options)
    {
    }
}
