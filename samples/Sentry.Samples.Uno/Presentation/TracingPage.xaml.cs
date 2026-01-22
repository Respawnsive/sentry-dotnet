using Microsoft.UI.Xaml.Controls;

namespace Sentry.Samples.Uno.Presentation;

public sealed partial class TracingPage : Page
{
    public TracingPage()
    {
        InitializeComponent();
        DataContext = new TracingViewModel();
    }
}
