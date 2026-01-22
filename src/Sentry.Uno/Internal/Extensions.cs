#if !PLATFORM_NEUTRAL
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
#endif

namespace Sentry.Uno.Internal;

internal static class Extensions
{
    public static void AddBreadcrumbForEvent(this IHub hub,
        SentryUnoOptions options,
        object? sender,
        string eventName,
        string? type,
        string? category,
        Action<Dictionary<string, string>>? addExtraData)
        => hub.AddBreadcrumbForEvent(options, sender, eventName, type, category, default, addExtraData);

    public static void AddBreadcrumbForEvent(this IHub hub,
        SentryUnoOptions options,
        object? sender,
        string eventName,
        string? type = null,
        string? category = null,
        BreadcrumbLevel level = default,
        Action<Dictionary<string, string>>? addExtraData = null)
    {
        var data = new Dictionary<string, string>();

#if !PLATFORM_NEUTRAL
        if (sender is FrameworkElement element)
        {
            data.AddElementInfo(options, element, null);
        }
#endif

        addExtraData?.Invoke(data);

        var message = sender != null ? $"{sender.ToStringOrTypeName()}.{eventName}" : eventName;
        hub.AddBreadcrumb(message, category, type, data, level);
    }

#if !PLATFORM_NEUTRAL
    public static void AddElementInfo(this IDictionary<string, string> data,
        SentryUnoOptions options,
        FrameworkElement? element,
        string? property)
    {
        if (element is null)
        {
            return;
        }

        var typeName = element.ToStringOrTypeName();
        var prefix = (property ?? typeName) + ".";

        if (property != null)
        {
            data.Add(property, typeName);
        }

        if (!string.IsNullOrWhiteSpace(element.Name))
        {
            data.Add(prefix + nameof(FrameworkElement.Name), element.Name);
        }

        var automationId = AutomationProperties.GetAutomationId(element);
        if (!string.IsNullOrWhiteSpace(automationId))
        {
            data.Add(prefix + "AutomationId", automationId);
        }

        if (options.IncludeTextInBreadcrumbs)
        {
            if (element is TextBlock { Text: { } text })
            {
                data.Add(prefix + nameof(TextBlock.Text), text);
            }
            else if (element is TextBox { Text: { } boxText })
            {
                data.Add(prefix + nameof(TextBox.Text), boxText);
            }
        }
    }
#endif

    public static string ToStringOrTypeName(this object o)
    {
        var t = o.GetType();
        var s = o.ToString();
        return s == null || s == t.FullName ? t.Name : s;
    }
}
