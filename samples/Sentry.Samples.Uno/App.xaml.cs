using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Sentry;
using Sentry.OpenTelemetry;
using Sentry.Uno;
using Uno.Resizetizer;

namespace Sentry.Samples.Uno;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    public IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseSentryUno(options =>
                {
                    // ============================================
                    // BASIC CONFIGURATION
                    // ============================================
                    options.Dsn = "https://36beb8ae739cb215fe4afc86255880cf@o4508993343979520.ingest.de.sentry.io/4510726484918352";

#if DEBUG
                    options.Debug = true;
                    options.DiagnosticLevel = SentryLevel.Debug;
                    options.TracesSampleRate = 1.0;
                    options.ProfilesSampleRate = 1.0;
                    options.SampleRate = 1.0F;
#else
                    options.SampleRate = 0.1F; // Sample 10% of events in production
                    options.TracesSampleRate = 0.1; // Sample 10% of traces in production
                    options.ProfilesSampleRate = 0.1; // Sample 10% of profiles in production
#endif

                    // ============================================
                    // ENRICHING EVENTS
                    // ============================================
                    // Modify events before they are sent
                    options.SetBeforeSend((@event, hint) =>
                    {
                        // Example: Filter out specific events
                        if (@event.Tags?.ContainsKey("FilterMe") == true)
                        {
                            return null; // Drop this event
                        }

                        // Example: Add custom tags
                        @event.SetTag("AppVersion", "1.0.0");
                        @event.SetTag("Platform", "Uno");

                        // Example: Modify fingerprinting (grouping)
                        var firstException = @event.SentryExceptions?.FirstOrDefault();
                        if (firstException?.Type == "System.NullReferenceException")
                        {
                            @event.SetFingerprint(new[] { "null-reference", firstException.Type });
                        }

                        return @event;
                    });

                    // Modify breadcrumbs before they are added
                    options.SetBeforeBreadcrumb((breadcrumb, hint) =>
                    {
                        // Example: Filter out sensitive breadcrumbs
                        if (breadcrumb.Message?.Contains("password", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return null; // Drop this breadcrumb
                        }

                        // Note: Breadcrumb.Data is readonly, so we can't modify it directly
                        // Custom data should be added when creating the breadcrumb

                        return breadcrumb;
                    });

                    // Configure scope (applied to all events)
                    options.ConfigureScope(scope =>
                    {
                        scope.SetTag("Application", "Sentry.Samples.Uno");
                        scope.SetTag("Framework", "Uno Platform");
                        scope.User = new SentryUser
                        {
                            Id = "sample-user-id",
                            Username = "sample-user",
                            Email = "sample@example.com"
                        };
                        scope.Contexts["device"] = new Dictionary<string, object>
                        {
                            { "name", "Sample Device" },
                            { "model", "Sample Model" }
                        };
                    });

                    // ============================================
                    // FINGERPRINTING
                    // ============================================
                    // Custom fingerprinting rules (see FingerprintingPage for examples)
                    // Fingerprinting is typically done in SetBeforeSend or via SetFingerprint on events

                    // ============================================
                    // LOGS / STRUCTURED LOGS
                    // ============================================
                    options.EnableLogs = true; // Enable structured logs
                    options.MinimumBreadcrumbLevel = LogLevel.Information;
                    options.MinimumEventLevel = LogLevel.Error;

                    // Configure structured logs
                    options.SetBeforeSendLog((log) =>
                    {
                        // Add custom attributes to logs
                        log.SetAttribute("AppName", "Sentry.Samples.Uno");
                        return log;
                    });

                    // Note: Log entry filtering is configured via Sentry.Extensions.Logging
                    // This would need to be configured separately if using structured logging

                    // ============================================
                    // TRACING
                    // ============================================
                    // Note: OpenTelemetry is configured separately in ConfigureServices
                    // For custom Sentry transactions, we use the default Sentry instrumenter
                    // OpenTelemetry is only used for automatic instrumentation (HTTP, etc.)
                    // Uncomment the line below ONLY if you want ALL transactions to use OpenTelemetry
                    // options.UseOpenTelemetry();

                    // ============================================
                    // PROFILING
                    // ============================================
                    // Profiling is enabled via ProfilesSampleRate above
                    // See ProfilingPage for examples of profiling-specific code

                    // ============================================
                    // EXTENDED CONFIGURATION
                    // ============================================
                    options.MaxBreadcrumbs = 100;
                    options.AttachStacktrace = true;
                    options.SendDefaultPii = false; // Set to true if you want to send PII (be careful!)
                    options.AttachScreenshot = true;
                    options.MaxAttachmentSize = 20 * 1024 * 1024; // 20 MB

                    // Release and environment
                    options.Release = "sentry-samples-uno@1.0.0";
                    options.Environment =
#if DEBUG
                        "development"
#else
                        "production"
#endif
                        ;

                    // In-app excludes/includes for stack traces
                    options.AddInAppExclude("Microsoft.");
                    options.AddInAppExclude("System.");
                    options.AddInAppInclude("Sentry.Samples.Uno");

                    // ============================================
                    // DATA MANAGEMENT
                    // ============================================
                    // Debug Information Files (symbols) are automatically uploaded
                    // when SentryUploadSymbols is enabled in the project file
                    // See Sentry.Samples.Uno.csproj for configuration

                    // Cache directory for offline storage
                    options.CacheDirectoryPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "Sentry",
                        "Cache"
                    );

                    // Maximum cache items
                    options.MaxCacheItems = 30;

                    // ============================================
                    // USER FEEDBACK
                    // ============================================
                    // User feedback is handled via the UserFeedbackPage
                    // See UserFeedbackPage for examples

                    // ============================================
                    // ADDITIONAL OPTIONS
                    // ============================================
                    options.AutoSessionTracking = true;
                    options.AutoSessionTrackingInterval = TimeSpan.FromSeconds(30);
                    options.SendClientReports = true;
                    options.CaptureFailedRequests = true;
                    options.FailedRequestStatusCodes = new List<HttpStatusCodeRange>
                    { 
                        HttpStatusCode.BadRequest, // Implicit conversion to HttpStatusCodeRange
                        HttpStatusCode.InternalServerError
                    };
                })
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning)

                        // Filter for Sentry.Samples.Uno namespace
                        .AddFilter("Sentry.Samples.Uno", LogLevel.Information);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                .UseHttp((context, services) =>
                {
#if DEBUG
                // DelegatingHandler will be automatically injected
                services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif

                })
                .ConfigureServices((context, services) =>
                {
                    // Configure OpenTelemetry tracing
                    // Note: OpenTelemetry may not work correctly on Android due to SDK native limitations
                    // The Android SDK shows "openTelemetryMode OFF" even when configured
                    // For reliable tracing on Android, use Sentry's native transactions (CustomTransaction, etc.)
                    // OpenTelemetry is kept here for demonstration purposes and may work better on other platforms
                    services.AddOpenTelemetry()
                        .WithTracing(tracerProviderBuilder =>
                        {
                            tracerProviderBuilder
                                .AddSource("Sentry.Samples.Uno")
                                .AddHttpClientInstrumentation()
                                .AddSentry(); // This sets the default propagator automatically
                        });
                })
                .UseNavigation(RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
        MainWindow.UseSentry(Host.Services);
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<MainPage, MainViewModel>(),
            new DataViewMap<SecondPage, SecondViewModel, Entity>(),
            new ViewMap<FingerprintingPage, FingerprintingViewModel>(),
            new ViewMap<LogsPage>(),
            new ViewMap<TracingPage, TracingViewModel>(),
            new ViewMap<ProfilingPage, ProfilingViewModel>(),
            new ViewMap<UserFeedbackPage, UserFeedbackViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested:
                [
                    new ("Main", View: views.FindByViewModel<MainViewModel>(), IsDefault:true),
                    new ("Second", View: views.FindByViewModel<SecondViewModel>()),
                    new ("Fingerprinting", View: views.FindByViewModel<FingerprintingViewModel>()),
                    new ("Logs", View: views.FindByView<LogsPage>()),
                    new ("Tracing", View: views.FindByViewModel<TracingViewModel>()),
                    new ("Profiling", View: views.FindByViewModel<ProfilingViewModel>()),
                    new ("UserFeedback", View: views.FindByViewModel<UserFeedbackViewModel>()),
                ]
            )
        );
    }
}
