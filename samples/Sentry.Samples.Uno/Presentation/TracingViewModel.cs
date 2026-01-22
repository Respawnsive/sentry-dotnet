using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sentry;

namespace Sentry.Samples.Uno.Presentation;

public partial class TracingViewModel : ObservableObject
{
    private static readonly ActivitySource ActivitySource = new("Sentry.Samples.Uno");

    [ObservableProperty]
    private string title = "Tracing";

    [RelayCommand]
    private static async Task OpenTelemetryTransaction()
    {
        // OpenTelemetry transaction example
        // Note: OpenTelemetry may not work on Android due to SDK native limitations
        // This is a demonstration - use CustomTransaction for reliable Sentry transactions
        using var activity = ActivitySource.StartActivity("OpenTelemetryTransaction");
        if (activity == null)
        {
            System.Diagnostics.Debug.WriteLine("[Sentry] Warning: ActivitySource.StartActivity returned null. OpenTelemetry may not be configured.");
            return;
        }
        
        activity.SetTag("operation.type", "custom");
        activity.SetTag("operation.name", "SampleOperation");

        // Simulate some work - longer delay
        await Task.Delay(500);

        // Create a child span
        using var childActivity = ActivitySource.StartActivity("ChildOperation");
        childActivity?.SetTag("child.operation", "Processing");
        await Task.Delay(300);
        
        System.Diagnostics.Debug.WriteLine($"[Sentry] OpenTelemetry transaction completed: {activity.TraceId}");
    }

    [RelayCommand]
    private static async Task OpenTelemetryHttp()
    {
        // HTTP requests are automatically instrumented when AddHttpClientInstrumentation is configured
        using var activity = ActivitySource.StartActivity("HttpRequest");
        
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://httpbin.org/get");
            activity?.SetTag("http.status_code", (int)response.StatusCode);
            activity?.SetTag("http.method", "GET");
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    [RelayCommand]
    private static async Task CustomTransaction()
    {
        // Custom Sentry transaction (using Sentry instrumenter, not OpenTelemetry)
        var transaction = SentrySdk.StartTransaction("custom-transaction", "sample-operation");
        
        try
        {
            // Simulate work - longer delay to ensure transaction is sent
            await Task.Delay(500);
            
            transaction.SetTag("custom.tag", "value");
            transaction.SetData("custom.data", "sample");
            
            // Log for debugging
            System.Diagnostics.Debug.WriteLine($"[Sentry] Custom transaction started: {transaction.TraceId}");
        }
        finally
        {
            transaction.Finish();
            System.Diagnostics.Debug.WriteLine($"[Sentry] Custom transaction finished: {transaction.TraceId}");
        }
    }

    [RelayCommand]
    private static async Task CustomSpan()
    {
        // Custom Sentry span
        var transaction = SentrySdk.StartTransaction("parent-transaction", "parent-operation");
        
        var span = transaction.StartChild("child-operation", "Custom span description");
        try
        {
            span.SetTag("span.tag", "value");
            // Longer delay to ensure transaction is sent
            await Task.Delay(300);
        }
        finally
        {
            span.Finish();
            transaction.Finish();
            System.Diagnostics.Debug.WriteLine($"[Sentry] Custom span transaction finished: {transaction.TraceId}");
        }
    }

    [RelayCommand]
    private static async Task NestedSpans()
    {
        // Example of nested spans
        var transaction = SentrySdk.StartTransaction("nested-transaction", "nested-operation");
        
        var outerSpan = transaction.StartChild("outer-operation", "Outer operation");
        try
        {
            // Longer delay to ensure transaction is sent
            await Task.Delay(300);
            
            var innerSpan = outerSpan.StartChild("inner-operation", "Inner operation");
            try
            {
                await Task.Delay(200);
                innerSpan.SetTag("inner.tag", "value");
            }
            finally
            {
                innerSpan.Finish();
            }
        }
        finally
        {
            outerSpan.Finish();
            transaction.Finish();
            System.Diagnostics.Debug.WriteLine($"[Sentry] Nested spans transaction finished: {transaction.TraceId}");
        }
    }
}
