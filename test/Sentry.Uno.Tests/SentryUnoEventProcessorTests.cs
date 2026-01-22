using Sentry.Uno.Internal;

namespace Sentry.Uno.Tests;

public class SentryUnoEventProcessorTests
{
    [Fact]
    public void Process_SetsSdkAndForeground()
    {
        var options = new SentryUnoOptions();
        var processor = new SentryUnoEventProcessor(options);
        var @event = new SentryEvent();

        SentryUnoEventProcessor.InForeground = true;

        var result = processor.Process(@event);

        result.Sdk.Name.Should().Be(Constants.SdkName);
        result.Sdk.Version.Should().Be(Constants.SdkVersion);
        result.Contexts.App.InForeground.Should().BeTrue();
    }
}
