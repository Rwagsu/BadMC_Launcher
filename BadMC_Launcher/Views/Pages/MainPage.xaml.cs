using System.Reflection.Metadata;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.ViewModels.Pages;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;

namespace BadMC_Launcher.Views.Pages;

public sealed partial class MainPage : Page {
    public MainPage() {
        this.InitializeComponent();
        DataContext = new MainPageViewModel();

        //Register NavigationToPage Messengers
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<Type>, string>(this, MainPageMessengerTokenEnum.PageNavigateToken.ToString(), MainFrameNavigate);
        WeakReferenceMessenger.Default.Register<RequestMessage<bool>, string>(this, MainPageMessengerTokenEnum.PageCloseToken.ToString(), MainFrameClose);
        WeakReferenceMessenger.Default.Register<RequestMessage<bool>, string>(this, MainPageMessengerTokenEnum.PageGoBackToken.ToString(), MainFrameGoBack);

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

    private void Button_Click(object sender, RoutedEventArgs e) {
        if (LaunchPadControl.IsOpen) {
            LaunchPadControl.IsOpen = false;
        }
        else {
            LaunchPadControl.IsOpen = true;
        }
    }
    private void OnLaunchPadKeyDownEsc(object sender, KeyRoutedEventArgs e) {
        if (e.Key == VirtualKey.Escape) {
            LaunchPadControl.IsOpen = false;
        }
    }
}
