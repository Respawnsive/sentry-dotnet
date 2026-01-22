using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Sentry.Uno.Internal;

#if !PLATFORM_NEUTRAL
using Microsoft.UI.Xaml;
#endif

namespace Sentry.Uno;

/// <summary>
/// Helpers to bind Sentry UI breadcrumbs to an Uno window.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SentryUnoWindowExtensions
{
#if !PLATFORM_NEUTRAL
    /// <summary>
    /// Binds Sentry breadcrumbs to the specified window.
    /// </summary>
    /// <param name="window">The window to bind.</param>
    /// <param name="services">The service provider used to resolve Sentry services.</param>
    public static void UseSentry(this Window window, IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(services);

        var binder = services.GetService<IUnoEventsBinder>();
        binder?.Bind(window);
    }
#else
    /// <summary>
    /// Binds Sentry breadcrumbs to the specified window.
    /// </summary>
    /// <remarks>
    /// Platform-neutral targets do not support Uno UI bindings.
    /// </remarks>
    public static void UseSentry(this object window, IServiceProvider services)
    {
        _ = window;
        _ = services;
    }
#endif
}
