using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sentry;

namespace Sentry.Samples.Uno.Presentation;

public partial class UserFeedbackViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "User Feedback";

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? email;

    [ObservableProperty]
    private string? message;

    [RelayCommand]
    private void SubmitFeedback()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            System.Diagnostics.Debug.WriteLine("[Sentry] Feedback submission skipped: message is empty");
            return;
        }

        var feedback = new SentryFeedback(Message, Email, Name);
        var feedbackId = SentrySdk.CaptureFeedback(feedback);
        
        System.Diagnostics.Debug.WriteLine($"[Sentry] Feedback submitted. ID: {feedbackId}, Message: {Message}, Email: {Email}, Name: {Name}");
        
        // Clear form
        Message = string.Empty;
        Name = string.Empty;
        Email = string.Empty;
    }

    [RelayCommand]
    private async Task SubmitFeedbackWithScreenshot()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            System.Diagnostics.Debug.WriteLine("[Sentry] Feedback submission skipped: message is empty");
            return;
        }

        // Note: Screenshot attachment would require platform-specific code
        // This is a simplified example
        var feedback = new SentryFeedback(Message, Email, Name);
        
        var hint = new SentryHint();
        // In a real app, you would attach a screenshot here
        // hint.AddAttachment(screenshotPath, AttachmentType.Default, "image/png");
        
        var feedbackId = SentrySdk.CaptureFeedback(feedback, hint: hint);
        
        System.Diagnostics.Debug.WriteLine($"[Sentry] Feedback with screenshot submitted. ID: {feedbackId}, Message: {Message}, Email: {Email}, Name: {Name}");
        
        // Clear form
        Message = string.Empty;
        Name = string.Empty;
        Email = string.Empty;
        
        await Task.CompletedTask;
    }
}
