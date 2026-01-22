namespace Sentry.Uno.Internal;

#if !PLATFORM_NEUTRAL
using Microsoft.UI.Xaml;
#endif

internal sealed class UnoWindowTracker
{
    public static UnoWindowTracker Current { get; } = new();

#if !PLATFORM_NEUTRAL
    public Window? CurrentWindow { get; private set; }
    public UIElement? RootElement { get; private set; }

    public void Update(Window window)
    {
        CurrentWindow = window;
        RootElement = window.Content;
    }
#else
    public void Update(object window)
    {
        _ = window;
    }
#endif
}
