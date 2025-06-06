using System.Reflection.Metadata;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.ViewModels.Pages;
using BadMC_Launcher.Views.UserControls;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Uno.Extensions;
using Uno.Extensions.Specialized;
using Windows.System;
using Windows.UI;

namespace BadMC_Launcher.Views.Pages;

public sealed partial class MainPage : Page {
    private MainPageViewModel viewModel => (MainPageViewModel)DataContext;
    public MainPage() {
        this.InitializeComponent();
        DataContext = new MainPageViewModel();

        //Register NavigationToPage Messengers
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<Type>, string>(this, MainPageMessengerTokenEnum.PageNavigateToken.ToString(), MainFrameNavigate);
        WeakReferenceMessenger.Default.Register<RequestMessage<bool>, string>(this, MainPageMessengerTokenEnum.PageCloseToken.ToString(), MainFrameClose);
        WeakReferenceMessenger.Default.Register<RequestMessage<bool>, string>(this, MainPageMessengerTokenEnum.PageGoBackToken.ToString(), MainFrameGoBack);

        //Register Notification Messengers
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<INotificationItem>, string>(this, MainPageMessengerTokenEnum.ShowNotificationToken.ToString(), ShowNotification);
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<(INotificationItem, Func<INotificationItem, UIElement, UIElement>)>, string>(this, MainPageMessengerTokenEnum.ShowNotificationToken.ToString(), ShowNotification);

        //Register GetXamlRoot Messengers
        WeakReferenceMessenger.Default.Register<RequestMessage<XamlRoot?>, string>(this, MainPageMessengerTokenEnum.XamlRootToken.ToString(), (r, m) => m.Reply(this.XamlRoot));
    }

    private void MainFrameNavigate(object recipient, ValueChangedMessage<Type> message) {
        if (MainFrame.Content != null && MainFrame.Content.GetType() == message.Value) {
            return;
        }

        if (typeof(Page).IsAssignableFrom(message.Value)) {
            MainFrame.Navigate(message.Value, null, new EntranceNavigationTransitionInfo());
        }
    }

    private void MainFrameClose(object recipient, RequestMessage<bool> message) {
        MainFrame.Content = null;
        MainFrame.BackStack.Clear();
        ClosePageButton.Visibility = Visibility.Collapsed;

#if !WINAPPSDK_PACKAGED
        MainFrameGrid.Visibility = Visibility.Collapsed;

        message.Reply(true);
#else
        message.Reply(false);
#endif
    }

    private void MainFrameGoBack(object recipient, RequestMessage<bool> message) {
        if (MainFrame.CanGoBack) {
            MainFrame.GoBack();
            message.Reply(true);
        }
        message.Reply(false);
    }

    private void ShowNotification(object recipient, ValueChangedMessage<INotificationItem> message) {
        if (message.Value is ToastMessageNotificationItem toastMessage) {
            var toastControl = new ToastMessageNotification() { NotificationItem = toastMessage };
            toastControl.NotificationHided += (s, e) => {
                if (s is UIElement element) {
                    NotificationsStackPanel.Children.Remove(element);
                }
            };
            NotificationsStackPanel.Children.Add(toastControl);
        }
        else if (message.Value is TipNotificationItem tipMessage) {
            var tipControl = new TipNotification() { NotificationItem = tipMessage };
            tipControl.NotificationHided += (s, e) => {
                if (s is UIElement element) {
                    NotificationsStackPanel.Children.Remove(element);
                }
            };
            NotificationsStackPanel.Children.Add(tipControl);
        }
        else if (message.Value is ProgressBarNotificationItem progressBarMessage) {
            var progressBarControl = new ProgressBarMessageNotification() { NotificationItem = progressBarMessage };
            progressBarControl.NotificationHided += (s, e) => {
                if (s is UIElement element) {
                    NotificationsStackPanel.Children.Remove(element);
                }
            };
            NotificationsStackPanel.Children.Add(progressBarControl);
        }
        else {
                App.GetService<NotificationService>().ShowNotification(new ToastMessageNotificationItem(
                    MessageSeverityEnum.Error,
                    App.GetService<ResourceLoader>().GetString("ToastNotification_NotificationNotFoundTitle"),
                    $"{message.Value.GetType().FullName ?? message.Value.GetType().ToString()} + {App.GetService<ResourceLoader>().GetString("ToastNotification_NotificationNotFoundMessage")}"));
        }
    }

    private void ShowNotification(object recipient, ValueChangedMessage<(INotificationItem, Func<INotificationItem, UIElement, UIElement>)> message) {
        var control = App.GetService<NotificationService>().GetNotificationControl(message.Value.Item1, message.Value.Item2);
        if (control == null) {
            App.GetService<NotificationService>().ShowNotification(new ToastMessageNotificationItem(
                MessageSeverityEnum.Error,
                App.GetService<ResourceLoader>().GetString("ToastNotification_NotificationNotFoundTitle"),
                $"{message.Value.GetType().FullName ?? message.Value.GetType().ToString()} + {App.GetService<ResourceLoader>().GetString("ToastNotification_NotificationNotFoundMessage")}"));
            return;
        }
        NotificationsStackPanel.Children.Add(control);
    }
}
