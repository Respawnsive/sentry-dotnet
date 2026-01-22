# Sentry.Uno

Uno Platform integration for Sentry.

## Installation

Reference `Sentry.Uno` from your Uno application project.

## Usage (Uno.Extensions.Hosting)

```csharp
public sealed partial class App : Application
{
    public IHost Host { get; private set; } = default!;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var appBuilder = this.CreateBuilder(args)
            .Configure(host => host.UseSentryUno(options =>
            {
                options.Dsn = "https://examplePublicKey@o0.ingest.sentry.io/0";
            }));

        var app = appBuilder.Build();
        Host = app.Host;
        MainWindow = app.Window;

        // Bind UI breadcrumbs (Window/Frame navigation)
        MainWindow.UseSentry(Host.Services);

        MainWindow.Activate();
    }
}
```

## Notes

- `UseSentryUno` registers the SDK and logging providers using the host builder.
- UI breadcrumbs are bound when you call `MainWindow.UseSentry(Host.Services)`.
- `AttachScreenshot` uses the bound window to capture screenshots.
- `SessionReplay.Mask` is an attached property for future session replay masking support.
- AndroidX: `Sentry.Uno` aligne explicitement les versions `Xamarin.AndroidX.Lifecycle.*` sur celles transitives de `Uno.WinUI` 6.4.242 pour éviter les erreurs NU1608/NU1605.