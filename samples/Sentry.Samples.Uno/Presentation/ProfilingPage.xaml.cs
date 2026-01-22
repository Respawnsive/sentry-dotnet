using Microsoft.UI.Xaml.Controls;

namespace Sentry.Samples.Uno.Presentation;

public sealed partial class ProfilingPage : Page
{
    public ProfilingPage()
    {
        InitializeComponent();
        DataContext = new ProfilingViewModel();
    }
}
