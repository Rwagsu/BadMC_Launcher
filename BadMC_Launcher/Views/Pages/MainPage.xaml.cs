using System.Reflection.Metadata;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.ViewModels.Pages;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;

namespace BadMC_Launcher.Views.Pages;

public sealed partial class MainPage : Page {
    public MainPage() {
        this.InitializeComponent();
        DataContext = new MainPageViewModel();

        //Register NavigationToPage Messengers
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<Type>, string>(this, MainPageMessengerTokenEnum.PageNavigateToken.ToString(), MainFrameNavigate);
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<object?>, string>(this, MainPageMessengerTokenEnum.PageCloseToken.ToString(), MainFrameClose);
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<object?>, string>(this, MainPageMessengerTokenEnum.PageGoBackToken.ToString(), MainFrameGoBack);

        //Register GetXamlRoot Messengers
        WeakReferenceMessenger.Default.Register<RequestMessage<XamlRoot?>, string>(this, MainPageMessengerTokenEnum.XamlRootToken.ToString(), (r, m) => m.Reply(this.XamlRoot));
    }

    private void MainFrameNavigate(object recipient, ValueChangedMessage<Type> message) {
        //MainSideBarFlyout.Hide();
        if (MainFrame.Content != null && MainFrame.Content.GetType() == message.Value) {
            return;
        }

        if (typeof(Page).IsAssignableFrom(message.Value)) {
            MainFrame.Navigate(message.Value, null, new EntranceNavigationTransitionInfo());
        }
    }

    private void MainFrameClose(object recipient, ValueChangedMessage<object?> _) {
        MainFrame.Content = null;
        MainFrame.BackStack.Clear();
        ClosePageButton.Visibility = Visibility.Collapsed;
    }

    private void MainFrameGoBack(object recipient, ValueChangedMessage<object?> _) {
        if (MainFrame.CanGoBack) {
            MainFrame.GoBack();
        }
    }
}
