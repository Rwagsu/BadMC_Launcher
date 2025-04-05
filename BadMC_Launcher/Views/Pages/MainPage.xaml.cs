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

        //Register GetXamlRoot Messengers
        WeakReferenceMessenger.Default.Register<RequestMessage<XamlRoot?>, string>(this, MainPageMessengerTokenEnum.XamlRootToken.ToString(), (r, m) => m.Reply(this.XamlRoot));
    }

    public void MainFrameNavigate(object recipient, ValueChangedMessage<Type> message) {
        //MainSideBarFlyout.Hide();
        if (MainSideBarFrame.Content != null && MainSideBarFrame.Content.GetType() == message.Value) {
            return;
        }

        if (typeof(Page).IsAssignableFrom(message.Value)) {
            MainSideBarFrame.Navigate(message.Value, null, new EntranceNavigationTransitionInfo());
        }
    }
}
