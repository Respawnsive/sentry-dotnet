using Microsoft.UI.Xaml.Controls;

namespace Sentry.Samples.Uno.Presentation;

public sealed partial class UserFeedbackPage : Page
{
    public UserFeedbackPage()
    {
        InitializeComponent();
        DataContext = new UserFeedbackViewModel();
    }
}
