using Sentry.Extensibility;

namespace Sentry.Uno.Internal;

internal sealed class SentryUnoEventProcessor : ISentryEventProcessor
{
    public static bool? InForeground { get; set; }

    private readonly SentryUnoOptions _options;

    public SentryUnoEventProcessor(SentryUnoOptions options)
    {
        _options = options;
    }

    public SentryEvent Process(SentryEvent @event)
    {
        @event.Sdk.Name = Constants.SdkName;
        @event.Sdk.Version = Constants.SdkVersion;
        @event.Contexts.Device.ApplyUnoDeviceData(_options.DiagnosticLogger);
        @event.Contexts.OperatingSystem.ApplyUnoOsData(_options.DiagnosticLogger);
        @event.Contexts.App.InForeground = InForeground;

        return @event;
    }
}
