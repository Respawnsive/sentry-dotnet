using System.Reflection;

namespace Sentry.Uno.Internal;

internal static class Constants
{
    // See: https://github.com/getsentry/sentry-release-registry
    public const string SdkName = "sentry.dotnet.uno";

    public static string SdkVersion = typeof(SentryUnoOptions).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}
