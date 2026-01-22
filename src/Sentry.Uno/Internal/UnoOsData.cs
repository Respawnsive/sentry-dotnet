using Sentry.Extensibility;
using OperatingSystem = Sentry.Protocol.OperatingSystem;

namespace Sentry.Uno.Internal;

internal static class UnoOsData
{
    public static void ApplyUnoOsData(this OperatingSystem os, IDiagnosticLogger? logger)
    {
        try
        {
            os.Version ??= Environment.OSVersion.Version.ToString();
            os.RawDescription ??= RuntimeInformation.OSDescription;

            if (System.OperatingSystem.IsWindows())
            {
                os.Name ??= "Windows";
            }
            else if (System.OperatingSystem.IsAndroid())
            {
                os.Name ??= "Android";
            }
            else if (System.OperatingSystem.IsIOS())
            {
                os.Name ??= "iOS";
            }
            else if (System.OperatingSystem.IsMacOS())
            {
                os.Name ??= "macOS";
            }
            else if (System.OperatingSystem.IsLinux())
            {
                os.Name ??= "Linux";
            }
            else
            {
                os.Name ??= "Unknown";
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error getting Uno OS information.");
        }
    }
}
