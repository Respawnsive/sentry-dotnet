using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sentry;

namespace Sentry.Samples.Uno.Presentation;

public partial class ProfilingViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Profiling";

    [RelayCommand]
    private static async Task StartProfiledTransaction()
    {
        // Profiling is automatically enabled when ProfilesSampleRate > 0
        // Transactions are automatically profiled
        // IMPORTANT: Transactions must be long enough (> 1 second) for profiling to work
        var transaction = SentrySdk.StartTransaction("profiled-operation", "profiling-sample");
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"[Sentry] Starting profiled transaction: {transaction.TraceId}");
            
            // Simulate CPU-intensive work - longer duration for profiling
            await Task.Run(() =>
            {
                for (int i = 0; i < 5000000; i++) // Increased iterations
                {
                    Math.Sqrt(i);
                }
            });
            
            // Additional delay to ensure transaction is long enough for profiling
            await Task.Delay(1500);
            
            transaction.SetTag("profiling.enabled", "true");
        }
        finally
        {
            transaction.Finish();
            System.Diagnostics.Debug.WriteLine($"[Sentry] Profiled transaction finished: {transaction.TraceId}");
        }
    }

    [RelayCommand]
    private static async Task ProfiledTransactionWithException()
    {
        // Profiling also works with exceptions
        // IMPORTANT: Transactions must be long enough (> 1 second) for profiling to work
        var transaction = SentrySdk.StartTransaction("profiled-with-exception", "profiling-sample");
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"[Sentry] Starting profiled transaction with exception: {transaction.TraceId}");
            
            // Longer delay to ensure transaction is long enough for profiling
            await Task.Delay(1500);
            
            // Simulate some work before exception
            await Task.Run(() =>
            {
                for (int i = 0; i < 2000000; i++)
                {
                    Math.Sqrt(i);
                }
            });
            
            throw new InvalidOperationException("Sample exception for profiling");
        }
        catch (Exception ex)
        {
            transaction.SetTag("exception.type", ex.GetType().Name);
            var eventId = SentrySdk.CaptureException(ex);
            System.Diagnostics.Debug.WriteLine($"[Sentry] Exception captured in profiled transaction. Event ID: {eventId}");
        }
        finally
        {
            transaction.Finish();
            System.Diagnostics.Debug.WriteLine($"[Sentry] Profiled transaction with exception finished: {transaction.TraceId}");
        }
    }
}
