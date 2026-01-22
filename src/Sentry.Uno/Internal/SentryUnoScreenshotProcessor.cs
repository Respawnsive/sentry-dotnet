using Sentry.Extensibility;

namespace Sentry.Uno.Internal;

internal sealed class SentryUnoScreenshotProcessor : ISentryEventProcessorWithHint
{
    private readonly SentryUnoOptions _options;

    public SentryUnoScreenshotProcessor(SentryUnoOptions options)
    {
        _options = options;
    }

    public SentryEvent? Process(SentryEvent @event)
    {
        return @event;
    }

    public SentryEvent? Process(SentryEvent @event, SentryHint hint)
    {
        if (!_options.BeforeCaptureInternal?.Invoke(@event, hint) ?? false)
        {
            return @event;
        }

        hint.Attachments.Add(new UnoScreenshotAttachment(_options));
        return @event;
    }
}
