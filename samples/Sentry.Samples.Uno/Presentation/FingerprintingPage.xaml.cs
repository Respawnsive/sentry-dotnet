using Microsoft.UI.Xaml.Controls;

namespace Sentry.Samples.Uno.Presentation;

public sealed partial class FingerprintingPage : Page
{
    public FingerprintingPage()
    {
        InitializeComponent();
        DataContext = new FingerprintingViewModel();
    }
}
