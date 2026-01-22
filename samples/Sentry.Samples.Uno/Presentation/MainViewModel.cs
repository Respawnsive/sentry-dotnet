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
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }
    public ICommand CaptureMessage { get; }
    public ICommand AddBreadcrumb { get; }
    public ICommand CaptureException { get; }

    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
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
