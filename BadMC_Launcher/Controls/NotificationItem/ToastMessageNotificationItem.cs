using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BadMC_Launcher.Controls.NotificationItem;

public partial class ToastMessageNotificationItem : ObservableObject, INotificationItem {
    public ToastMessageNotificationItem() {
        // Default constructor
        Title = string.Empty;
        Message = string.Empty;

        // Default severity
        NotificationSeverity = MessageSeverityEnum.Info;

        // Default values
        switch (NotificationSeverity) {
            case MessageSeverityEnum.Info:
                // Info icon
                NotificationIcon = new FontIconSource { Glyph = "\uF167" };
                break;
            case MessageSeverityEnum.Important:
                // Important icon
                NotificationIcon = new FontIconSource { Glyph = "\uE917" };
                break;
            case MessageSeverityEnum.Success:
                // Success icon
                NotificationIcon = new FontIconSource { Glyph = "\uEC61" };
                break;
            case MessageSeverityEnum.Warning:
                // Warning icon
                NotificationIcon = new FontIconSource { Glyph = "\uF167" };
                break;
            case MessageSeverityEnum.Error:
                // Error icon
                NotificationIcon = new FontIconSource { Glyph = "\uEB90" };
                break;
            default:
                // Default icon
                NotificationIcon = new FontIconSource() { Glyph = "\uF167" };
                break;
        }

        NotificationColor = NotificationSeverity switch {
            MessageSeverityEnum.Info => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
            MessageSeverityEnum.Important => (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"],
            MessageSeverityEnum.Success => (Brush)Application.Current.Resources["SystemFillColorSuccessBrush"],
            MessageSeverityEnum.Warning => (Brush)Application.Current.Resources["SystemFillColorCautionBrush"],
            MessageSeverityEnum.Error => (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"],
            _ => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
        };
    }

    public event Action? HideExecuteAction;

    // Notification title
    [ObservableProperty]
    public partial string Title { get; set; }

    // Notification message
    [ObservableProperty]
    public partial string Message { get; set; }

    // Notification severity
    [ObservableProperty]
    public partial MessageSeverityEnum NotificationSeverity { get; set; }

    // Notification icon
    [ObservableProperty]
    public partial IconSource NotificationIcon { get; set; }

    // Notification color
    [ObservableProperty]
    public partial Brush NotificationColor { get; set; }

    // Primary button properties
    [ObservableProperty]
    public partial ButtonBase? PrimaryActionButton { get; set; }

    // Secondary button properties
    [ObservableProperty]
    public partial ButtonBase? SecondaryActionButton { get; set; }

    public void InvokeHideExecuteAction() {
        HideExecuteAction?.Invoke();
    }

    public void Close() {
        App.GetService<NotificationService>().CloseNotification(this);
    }
}
