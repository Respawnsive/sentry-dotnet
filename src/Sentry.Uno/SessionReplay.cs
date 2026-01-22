#if !PLATFORM_NEUTRAL
using Microsoft.UI.Xaml;
#endif

namespace Sentry.Uno;

/// <summary>
/// Attached properties used to control session replay masking for Uno UI elements.
/// </summary>
public static class SessionReplay
{
#if !PLATFORM_NEUTRAL
    /// <summary>
    /// Mask can be used to either unmask or mask a view.
    /// </summary>
    public static readonly DependencyProperty MaskProperty =
        DependencyProperty.RegisterAttached(
            "Mask",
            typeof(SessionReplayMaskMode),
            typeof(SessionReplay),
            new PropertyMetadata(SessionReplayMaskMode.Mask));

    /// <summary>
    /// Gets the value of the Mask property for a view.
    /// </summary>
    public static SessionReplayMaskMode GetMask(DependencyObject view) =>
        (SessionReplayMaskMode)view.GetValue(MaskProperty);

    /// <summary>
    /// Sets the value of the Mask property for a view.
    /// </summary>
    /// <param name="view">The view element to mask or unmask.</param>
    /// <param name="value">The value to assign.</param>
    public static void SetMask(DependencyObject view, SessionReplayMaskMode value) =>
        view.SetValue(MaskProperty, value);
#else
    /// <summary>
    /// Gets the value of the Mask property for a view.
    /// </summary>
    public static SessionReplayMaskMode GetMask(object view)
    {
        _ = view;
        return SessionReplayMaskMode.Mask;
    }

    /// <summary>
    /// Sets the value of the Mask property for a view.
    /// </summary>
    public static void SetMask(object view, SessionReplayMaskMode value)
    {
        _ = view;
        _ = value;
    }
#endif
}
