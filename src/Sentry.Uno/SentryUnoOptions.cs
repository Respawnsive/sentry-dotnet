using Sentry.Extensions.Logging;

namespace Sentry.Uno;

/// <summary>
/// Sentry Uno integration options.
/// </summary>
public class SentryUnoOptions : SentryLoggingOptions
{
    /// <summary>
    /// Creates a new instance of <see cref="SentryUnoOptions"/>.
    /// </summary>
    public SentryUnoOptions()
    {
        AutoSessionTracking = true;
        DetectStartupTime = StartupTimeDetectionMode.Fast;
        IsEnvironmentUser = false;
    }

    /// <summary>
    /// Gets or sets whether elements that expose text content
    /// will have their text included on breadcrumbs.
    /// Use caution when enabling, as such values may contain PII.
    /// The default is <c>false</c> (exclude).
    /// </summary>
    public bool IncludeTextInBreadcrumbs { get; set; }

    /// <summary>
    /// Gets or sets whether elements that expose a title
    /// will have their titles included on breadcrumbs.
    /// Use caution when enabling, as such values may contain PII.
    /// The default is <c>false</c> (exclude).
    /// </summary>
    public bool IncludeTitleInBreadcrumbs { get; set; }

    /// <summary>
    /// Automatically attaches a screenshot of the app at the time of the event capture.
    /// </summary>
    /// <remarks>
    /// Make sure to only enable this feature if no sensitive data, such as PII, can be visible on the screen.
    /// Screenshots can be removed from some specific events during BeforeSend through the Hint.
    /// </remarks>
    public bool AttachScreenshot { get; set; }

    private Func<SentryEvent, SentryHint, bool>? _beforeCapture;

    /// <summary>
    /// Action performed before attaching a screenshot.
    /// </summary>
    internal Func<SentryEvent, SentryHint, bool>? BeforeCaptureInternal => _beforeCapture;

    /// <summary>
    /// Configures a callback function to be invoked before taking a screenshot.
    /// </summary>
    /// <remarks>
    /// If this callback returns false the capture will not take place.
    /// </remarks>
    /// <param name="beforeCapture">Callback to be executed before taking a screenshot</param>
    public void SetBeforeScreenshotCapture(Func<SentryEvent, SentryHint, bool> beforeCapture)
    {
        _beforeCapture = beforeCapture;
    }
}
