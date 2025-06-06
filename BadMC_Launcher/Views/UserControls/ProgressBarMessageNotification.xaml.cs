using System.Numerics;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Enums;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class ProgressBarMessageNotification : UserControl {
    // Register property
    public static readonly DependencyProperty NotificationItemProperty = DependencyProperty.Register(
         nameof(NotificationItem),
         typeof(ProgressBarNotificationItem),
         typeof(ProgressBarMessageNotification),
         new PropertyMetadata(null)
    );

    public ProgressBarMessageNotification() {
        this.InitializeComponent();
    }

    public event EventHandler? NotificationHided;

    // Properties
    public ProgressBarNotificationItem? NotificationItem {
        get => (ProgressBarNotificationItem?)GetValue(NotificationItemProperty);
        set => SetValue(NotificationItemProperty, value);
    }

#if WINAPPSDK_PACKAGED
    // WinAppSDK use async
    private async void OnControlLoaded(object sender, RoutedEventArgs e) {
#else
    // Non-WinAppSDK use sync
    private void OnControlLoaded(object sender, RoutedEventArgs e) {
#endif

        if (NotificationItem != null) {
#if WINAPPSDK_PACKAGED
            NotificationItem.HideExecuteAction += async () => {
                await CloseNotificationAnimation.StartAsync();
                NotificationHided?.Invoke(this, new EventArgs());
            };
#endif

#if WINAPPSDK_PACKAGED
            await OpenNotificationAnimation.StartAsync();
#else
                Translation = new Vector3(-430, 0, 0);
#endif
            IsHitTestVisible = true;
        }
    }

#if WINAPPSDK_PACKAGED
    // WinAppSDK use async
    private async void OnCloseButtonClick(object sender, RoutedEventArgs e) {
#else
    // Non-WinAppSDK use sync
    private void OnCloseButtonClick(object sender, RoutedEventArgs e) {
#endif
        //IsHitTestVisible = false;
#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif

        NotificationHided?.Invoke(this, new EventArgs());
    }

    private bool GetProgressBarState(ProgressBarStateEnum progressBarState, string propertyName) {
        return propertyName switch {
            "IsIndeterminate" => progressBarState == ProgressBarStateEnum.Running ? true : false,
            "ShowPaused" => progressBarState == ProgressBarStateEnum.Pause ? true : false,
            "ShowError" => progressBarState == ProgressBarStateEnum.Error ? true : false,
            _ => false
        };
    }

    private Visibility IsStringWhiteSpaceOrEmpty(string? parameter) => string.IsNullOrWhiteSpace(parameter) ? Visibility.Collapsed : Visibility.Visible;
}
