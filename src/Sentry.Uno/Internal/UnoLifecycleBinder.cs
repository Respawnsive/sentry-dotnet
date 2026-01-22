#if !PLATFORM_NEUTRAL
using Microsoft.UI.Xaml;
using UnoApplication = Microsoft.UI.Xaml.Application;
#endif

namespace Sentry.Uno.Internal;

#if !PLATFORM_NEUTRAL
internal interface IUnoLifecycleBinder
{
    void Bind();
}

internal sealed class UnoLifecycleBinder : IUnoLifecycleBinder
{
    private bool _bound;

    public void Bind()
    {
        if (_bound)
        {
            return;
        }

        var app = UnoApplication.Current;
        if (app is null)
        {
            return;
        }

        SentryUnoEventProcessor.InForeground = true;
        _bound = true;
    }
}
#else
internal interface IUnoLifecycleBinder
{
    void Bind();
}

internal sealed class UnoLifecycleBinder : IUnoLifecycleBinder
{
    public void Bind()
    {
    }
}
#endif
