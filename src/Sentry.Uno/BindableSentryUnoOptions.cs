using Sentry.Extensions.Logging;

namespace Sentry.Uno;

/// <inheritdoc cref="BindableSentryOptions"/>
internal class BindableSentryUnoOptions : BindableSentryLoggingOptions
{
    public bool? IncludeTextInBreadcrumbs { get; set; }
    public bool? IncludeTitleInBreadcrumbs { get; set; }
    public bool? AttachScreenshot { get; set; }

    public void ApplyTo(SentryUnoOptions options)
    {
        base.ApplyTo(options);
        options.IncludeTextInBreadcrumbs = IncludeTextInBreadcrumbs ?? options.IncludeTextInBreadcrumbs;
        options.IncludeTitleInBreadcrumbs = IncludeTitleInBreadcrumbs ?? options.IncludeTitleInBreadcrumbs;
        options.AttachScreenshot = AttachScreenshot ?? options.AttachScreenshot;
    }
}
