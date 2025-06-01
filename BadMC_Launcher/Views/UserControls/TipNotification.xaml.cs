using System.Numerics;
using BadMC_Launcher.Controls.NotificationItem;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class TipNotification : UserControl {
    // Register property
    public static readonly DependencyProperty NotificationItemProperty = DependencyProperty.Register(
         nameof(NotificationItem),
         typeof(TipNotificationItem),
         typeof(TipNotification),
         new PropertyMetadata(null)
    );

    public TipNotification() {
		this.InitializeComponent();

        DelayedHide();
    }

    public event EventHandler? NotificationHided;

    // Properties
    public TipNotificationItem? NotificationItem {
        get => (TipNotificationItem?)GetValue(NotificationItemProperty);
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


    public async void DelayedHide() {
        // Wait for 5 seconds before hiding
        await Task.Delay(5000);

#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif

        NotificationHided?.Invoke(this, new EventArgs());
    }
}
