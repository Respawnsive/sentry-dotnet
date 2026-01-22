using Sentry.Extensibility;
using Sentry.Protocol;
using Device = Sentry.Protocol.Device;

namespace Sentry.Uno.Internal;

internal static class UnoDeviceData
{
    public static void ApplyUnoDeviceData(this Device device, IDiagnosticLogger? logger)
    {
        try
        {
            device.Name ??= Environment.MachineName;
            device.Architecture ??= RuntimeInformation.OSArchitecture.ToString();
            device.Model ??= RuntimeInformation.OSDescription;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error getting Uno device information.");
        }
    }
}
