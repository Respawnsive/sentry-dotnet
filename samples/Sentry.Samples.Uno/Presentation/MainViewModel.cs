using CommunityToolkit.Mvvm.Input;
using Sentry;

namespace Sentry.Samples.Uno.Presentation;

public partial class MainViewModel : ObservableObject
{
    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    public MainViewModel(
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {appInfo?.Value?.Environment}";
        GoToSecond = new AsyncRelayCommand(GoToSecondView);
        CaptureMessage = new RelayCommand(CaptureSentryMessage);
        AddBreadcrumb = new RelayCommand(AddSentryBreadcrumb);
        CaptureException = new RelayCommand(CaptureSentryException);
        NavigateToFingerprinting = new AsyncRelayCommand(NavigateToFingerprintingView);
        NavigateToLogs = new AsyncRelayCommand(async () => await _navigator.NavigateViewAsync<LogsPage>(this));
        NavigateToTracing = new AsyncRelayCommand(NavigateToTracingView);
        NavigateToProfiling = new AsyncRelayCommand(NavigateToProfilingView);
        NavigateToUserFeedback = new AsyncRelayCommand(NavigateToUserFeedbackView);
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }
    public ICommand CaptureMessage { get; }
    public ICommand AddBreadcrumb { get; }
    public ICommand CaptureException { get; }
    public ICommand NavigateToFingerprinting { get; }
    public ICommand NavigateToLogs { get; set; }
    public ICommand NavigateToTracing { get; }
    public ICommand NavigateToProfiling { get; }
    public ICommand NavigateToUserFeedback { get; }

    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    }

    private async Task NavigateToFingerprintingView()
    {
        await _navigator.NavigateViewModelAsync<FingerprintingViewModel>(this);
    }


    private async Task NavigateToTracingView()
    {
        await _navigator.NavigateViewModelAsync<TracingViewModel>(this);
    }

    private async Task NavigateToProfilingView()
    {
        await _navigator.NavigateViewModelAsync<ProfilingViewModel>(this);
    }

    private async Task NavigateToUserFeedbackView()
    {
        await _navigator.NavigateViewModelAsync<UserFeedbackViewModel>(this);
    }

    private static void CaptureSentryMessage()
    {
        SentrySdk.CaptureMessage("Hello from Sentry.Samples.Uno");
    }

    private static void AddSentryBreadcrumb()
    {
        SentrySdk.AddBreadcrumb("Breadcrumb from Sentry.Samples.Uno");
    }

    private static void CaptureSentryException()
    {
        var exception = new InvalidOperationException("Sample exception from Sentry.Samples.Uno");
        SentrySdk.CaptureException(exception);
    }

}
