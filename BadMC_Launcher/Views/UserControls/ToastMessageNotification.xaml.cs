using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Numerics;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.NotificationItem;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BadMC_Launcher.Views.UserControls;

public sealed partial class ToastMessageNotification : UserControl {
    // Register property
    public static readonly DependencyProperty NotificationItemProperty = DependencyProperty.Register(
         nameof(NotificationItem),
         typeof(ToastMessageNotificationItem),
         typeof(ToastMessageNotification),
         new PropertyMetadata(null)
    );

    public ToastMessageNotification() {
        this.InitializeComponent();
        DelayedHide();
    }

    public event EventHandler? NotificationHided;

    // Properties
    public ToastMessageNotificationItem? NotificationItem {
        get => (ToastMessageNotificationItem?)GetValue(NotificationItemProperty);
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
        await Task.Delay(7000);
        if (NotificationItem?.PrimaryActionButton != null || NotificationItem?.SecondaryActionButton != null) {
            // If there are buttons, do not auto-hide
            return;
        }

#if WINAPPSDK_PACKAGED
        await CloseNotificationAnimation.StartAsync();
#endif

        NotificationHided?.Invoke(this, new EventArgs());
    }

    private Visibility IsStringWhiteSpaceOrEmpty(string? parameter) => string.IsNullOrWhiteSpace(parameter) ? Visibility.Collapsed : Visibility.Visible;
}
