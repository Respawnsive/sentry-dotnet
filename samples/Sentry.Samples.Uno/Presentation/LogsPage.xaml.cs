using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Sentry;

namespace Sentry.Samples.Uno.Presentation;

public sealed partial class LogsPage : Page
{
    public LogsPage()
    {
        InitializeComponent();
    }

    private ILogger<LogsPage>? GetLogger()
    {
        // Try to get logger from application host
        if (Application.Current is App app && app.Host != null)
        {
            return app.Host.Services.GetService<ILogger<LogsPage>>();
        }
        return null;
    }

    private void OnLogInformation(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GetLogger()?.LogInformation("User performed an action: {Action}", "ButtonClicked");
    }

    private void OnLogWarning(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GetLogger()?.LogWarning("Potential issue detected: {Issue}", "LowMemory");
    }

    private void OnLogError(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GetLogger()?.LogError("An error occurred: {Error}", "DatabaseConnectionFailed");
    }

    private void OnLogCritical(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GetLogger()?.LogCritical("Critical system failure: {Failure}", "ServiceUnavailable");
    }

    private void OnLogStructured(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        GetLogger()?.LogInformation(
            "User {UserId} performed {Action} on {Resource}",
            12345,
            "Delete",
            "Document-123"
        );
    }

    private void OnLogWithException(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            throw new InvalidOperationException("Sample exception for logging");
        }
        catch (Exception ex)
        {
            GetLogger()?.LogError(ex, "An exception occurred while processing: {Operation}", "DataProcessing");
        }
    }
}
