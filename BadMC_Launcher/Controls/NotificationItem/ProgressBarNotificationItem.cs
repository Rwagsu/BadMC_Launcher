using System;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace BadMC_Launcher.Controls.NotificationItem;

public partial class ProgressBarNotificationItem : ObservableObject, INotificationItem {
    public ProgressBarNotificationItem() {
        // Default constructor
        Title = string.Empty;
        Message = string.Empty;
        ProgressTitle = string.Empty;
        ProgressString = string.Empty;
        ProgressValue = 0;
        ProgressStatus = string.Empty;

        // Default severity
        NotificationState = ProgressBarStateEnum.Running;

        // Default values
        NotificationIcon = new FontIconSource { Glyph = "\uF167" };

        // Default color
        NotificationColor = NotificationState switch {
            ProgressBarStateEnum.Running => (Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
            ProgressBarStateEnum.Pause => (Brush)Application.Current.Resources["SystemFillColorCautionBrush"],
            ProgressBarStateEnum.Error => (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"],
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

    // Notification progress
    [ObservableProperty]
    public partial string ProgressTitle { get; set; }

    [ObservableProperty]
    public partial string ProgressString { get; set; }

    [ObservableProperty]
    public partial int ProgressValue { get; set; }

    [ObservableProperty]
    public partial string ProgressStatus { get; set; }

    // Notification severity
    [ObservableProperty]
    public partial ProgressBarStateEnum NotificationState { get; set; }

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

    async partial void OnProgressValueChanged(int value) {
        if (value >= 100) {
            await Task.Delay(5000);
            HideExecuteAction?.Invoke();
        }
    }
}
