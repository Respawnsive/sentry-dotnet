#if !PLATFORM_NEUTRAL
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media;
#endif

namespace Sentry.Uno.Internal;

#if !PLATFORM_NEUTRAL
internal interface IUnoEventsBinder
{
    void Bind(Window window);
}

internal sealed class UnoEventsBinder : IUnoEventsBinder
{
    private readonly IHub _hub;
    private readonly SentryUnoOptions _options;
    private readonly UnoWindowTracker _windowTracker;
    private Window? _window;
    private Frame? _frame;

    internal const string NavigationType = "navigation";
    internal const string SystemType = "system";
    internal const string LifecycleCategory = "ui.lifecycle";
    internal const string NavigationCategory = "navigation";

    public UnoEventsBinder(IHub hub, IOptions<SentryUnoOptions> options, UnoWindowTracker windowTracker)
    {
        _hub = hub;
        _options = options.Value;
        _windowTracker = windowTracker;
    }

    public void Bind(Window window)
    {
        Unbind();
        _window = window;
        _windowTracker.Update(window);

        window.Activated += OnWindowActivated;
        window.Closed += OnWindowClosed;

        if (window.Content is FrameworkElement root)
        {
            root.Loaded += OnRootLoaded;
            root.Unloaded += OnRootUnloaded;
            BindFrameFromRoot(root);
        }
    }

    private void Unbind()
    {
        if (_window is null)
        {
            return;
        }

        _window.Activated -= OnWindowActivated;
        _window.Closed -= OnWindowClosed;

        if (_window.Content is FrameworkElement root)
        {
            root.Loaded -= OnRootLoaded;
            root.Unloaded -= OnRootUnloaded;
        }

        UnbindFrame();
        _window = null;
    }

    private void BindFrameFromRoot(FrameworkElement root)
    {
        var frame = root as Frame ?? FindFirstDescendant<Frame>(root);
        if (frame is null)
        {
            return;
        }

        _frame = frame;
        _frame.Navigated += OnFrameNavigated;
        _frame.Navigating += OnFrameNavigating;
    }

    private void UnbindFrame()
    {
        if (_frame is null)
        {
            return;
        }

        _frame.Navigated -= OnFrameNavigated;
        _frame.Navigating -= OnFrameNavigating;
        _frame = null;
    }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement root)
        {
            BindFrameFromRoot(root);
        }
    }

    private void OnRootUnloaded(object sender, RoutedEventArgs e)
    {
        UnbindFrame();
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
    {
        _hub.AddBreadcrumbForEvent(_options, sender, nameof(Window.Activated), SystemType, LifecycleCategory, data =>
            data.Add(nameof(e.WindowActivationState), e.WindowActivationState.ToString()));
    }

    private void OnWindowClosed(object sender, WindowEventArgs e)
    {
        _hub.AddBreadcrumbForEvent(_options, sender, nameof(Window.Closed), SystemType, LifecycleCategory);
    }

    private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
    {
        _hub.AddBreadcrumbForEvent(_options, sender, nameof(Frame.Navigating), NavigationType, NavigationCategory, data =>
        {
            data.Add(nameof(e.SourcePageType), e.SourcePageType?.Name ?? "<null>");
            data.Add(nameof(e.NavigationMode), e.NavigationMode.ToString());
        });
    }

    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        _hub.AddBreadcrumbForEvent(_options, sender, nameof(Frame.Navigated), NavigationType, NavigationCategory, data =>
        {
            data.Add(nameof(e.SourcePageType), e.SourcePageType?.Name ?? "<null>");
            data.Add(nameof(e.NavigationMode), e.NavigationMode.ToString());
        });
    }

    private static T? FindFirstDescendant<T>(DependencyObject root) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                return match;
            }

            var descendant = FindFirstDescendant<T>(child);
            if (descendant is not null)
            {
                return descendant;
            }
        }

        return default;
    }
}
#else
internal interface IUnoEventsBinder
{
    void Bind(object window);
}

internal sealed class UnoEventsBinder : IUnoEventsBinder
{
    public void Bind(object window)
    {
        _ = window;
    }
}
#endif
