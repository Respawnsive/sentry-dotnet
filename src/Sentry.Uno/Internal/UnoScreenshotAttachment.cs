using System.Threading;
using Sentry;
#if !PLATFORM_NEUTRAL
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Sentry.Uno.Internal;

internal sealed class UnoScreenshotAttachment : SentryAttachment
{
    public UnoScreenshotAttachment(SentryUnoOptions options)
        : this(
            AttachmentType.Default,
            new UnoScreenshotAttachmentContent(options),
            "screenshot.png",
            "image/png")
    {
    }

    private UnoScreenshotAttachment(
        AttachmentType type,
        IAttachmentContent content,
        string fileName,
        string? contentType)
        : base(type, content, fileName, contentType)
    {
    }
}

internal sealed class UnoScreenshotAttachmentContent : IAttachmentContent
{
    private readonly SentryUnoOptions _options;

    public UnoScreenshotAttachmentContent(SentryUnoOptions options)
    {
        _options = options;
    }

    public Stream GetStream()
    {
        try
        {
            return GetStreamInternal();
        }
        catch (Exception ex)
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Error, "Failed to capture Uno screenshot", ex);
            return new MemoryStream();
        }
    }

    private Stream GetStreamInternal()
    {
#if !PLATFORM_NEUTRAL
        var tracker = UnoWindowTracker.Current;
        var window = tracker.CurrentWindow;
        var root = tracker.RootElement;

        if (window is null || root is null)
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, "Skipping screenshot capture because no window/root element was registered.");
            return Stream.Null;
        }

        if (window.DispatcherQueue.HasThreadAccess)
        {
            return CaptureOnUiThread(root);
        }

        Stream? stream = null;
        using var resetEvent = new ManualResetEventSlim(false);
        window.DispatcherQueue.TryEnqueue(() =>
        {
            stream = CaptureOnUiThread(root);
            resetEvent.Set();
        });
        resetEvent.Wait();

        return stream ?? Stream.Null;
#else
        _options.DiagnosticLogger?.Log(SentryLevel.Debug, "Screenshot capture is not available on platform-neutral targets.");
        return Stream.Null;
#endif
    }

#if !PLATFORM_NEUTRAL
    private static Stream CaptureOnUiThread(Microsoft.UI.Xaml.UIElement root)
    {
        var renderTarget = new RenderTargetBitmap();
        renderTarget.RenderAsync(root).AsTask().GetAwaiter().GetResult();

        var pixelBuffer = renderTarget.GetPixelsAsync().AsTask().GetAwaiter().GetResult();
        var width = (uint)renderTarget.PixelWidth;
        var height = (uint)renderTarget.PixelHeight;

        var stream = new InMemoryRandomAccessStream();
        var encoder = BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream).AsTask().GetAwaiter().GetResult();
        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            width,
            height,
            96,
            96,
            pixelBuffer.ToArray());
        encoder.FlushAsync().AsTask().GetAwaiter().GetResult();
        stream.Seek(0);

        return stream.AsStreamForRead();
    }
#endif
}
