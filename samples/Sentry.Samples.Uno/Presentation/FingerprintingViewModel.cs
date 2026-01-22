using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sentry;

namespace Sentry.Samples.Uno.Presentation;

public partial class FingerprintingViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Fingerprinting";

    [RelayCommand]
    private static void CustomFingerprintByType()
    {
        // Example: Group all NullReferenceException together regardless of stack trace
        var exception = new NullReferenceException("Sample null reference");
        SentrySdk.CaptureException(exception, scope =>
        {
            scope.SetFingerprint(new[] { "null-reference", exception.GetType().Name });
        });
    }

    [RelayCommand]
    private static void CustomFingerprintByMessage()
    {
        // Example: Group events by message content
        SentrySdk.CaptureMessage("Database connection failed", scope =>
        {
            scope.Level = SentryLevel.Error;
            scope.SetFingerprint(new[] { "database-error", "connection-failed" });
        });
    }

    [RelayCommand]
    private static void CustomFingerprintMultiple()
    {
        // Example: Multiple fingerprint values for complex grouping
        var exception = new InvalidOperationException("Operation failed");
        SentrySdk.CaptureException(exception, scope =>
        {
            scope.SetTag("Component", "PaymentProcessor");
            scope.SetTag("ErrorCode", "PAY001");
            scope.SetFingerprint(new[] {
                "payment-error",
                exception.GetType().Name,
                scope.Tags.GetValueOrDefault("ErrorCode", "unknown")
            });
        });
    }

    [RelayCommand]
    private static void DefaultFingerprint()
    {
        // Example: Use default Sentry fingerprinting (automatic grouping)
        var exception = new ArgumentException("Invalid argument provided");
        SentrySdk.CaptureException(exception);
    }
}
