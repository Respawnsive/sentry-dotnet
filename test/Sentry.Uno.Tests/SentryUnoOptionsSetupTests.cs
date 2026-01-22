using Microsoft.Extensions.Configuration;
using Sentry.Uno.Internal;

namespace Sentry.Uno.Tests;

public class SentryUnoOptionsSetupTests
{
    [Fact]
    public void Configure_BindsAndSetsDefaults()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Sentry:Debug"] = "true",
                ["Sentry:IncludeTextInBreadcrumbs"] = "true",
                ["Sentry:IncludeTitleInBreadcrumbs"] = "true",
                ["Sentry:AttachScreenshot"] = "true",
            })
            .Build();

        var setup = new SentryUnoOptionsSetup(config);
        var options = new SentryUnoOptions();

        setup.Configure(options);

        options.InitializeSdk.Should().BeFalse();
        options.IsGlobalModeEnabled.Should().BeTrue();
        options.DiagnosticLogger.Should().NotBeNull();
        options.IncludeTextInBreadcrumbs.Should().BeTrue();
        options.IncludeTitleInBreadcrumbs.Should().BeTrue();
        options.AttachScreenshot.Should().BeTrue();
    }
}
