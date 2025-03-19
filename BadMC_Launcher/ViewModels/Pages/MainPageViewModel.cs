using System.Collections.ObjectModel;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.Servicess.Settings;
using BadMC_Launcher.Views.Pages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Uno.UI.RemoteControl;
using BadMC_Launcher.Classes.ViewClasses;
using BadMC_Launcher.Enums.MessengerTokenEnum;

namespace BadMC_Launcher.ViewModels.Pages;

public partial class MainPageViewModel : ObservableObject {


    public bool MainSideBarFrameCanGoBack { get; set; }

    public MainPageViewModel() {
        //Init Property
        WindowName = App.GetService<ThemeSettingService>().WindowName;
        MainSideBarToolVisibility = Visibility.Collapsed;
        MainSideBarItems = MainSideBarData.MainSideBarItems;
        MainSideBarFooterItems = MainSideBarData.MainSideBarFooterItems;
        App.GetService<ThemeSettingService>().SetBackground((brush) => {
        AppBackground = brush;
        });
    }

    [ObservableProperty]
    public partial string? WindowName { get; set; }
    
    [ObservableProperty]
    public partial Brush? AppBackground { get; set; }

    //MainSideBar Items
    [ObservableProperty]
    public partial ObservableCollection<MainSideBarItem> MainSideBarItems { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<MainSideBarItem> MainSideBarFooterItems { get; set; }

    [ObservableProperty]
    public partial MainSideBarItem? MainSideBarSelectedItem { get; set; }

    [ObservableProperty]
    public partial UIElement? MainSideBarFlyoutContent { get; set; }

    [ObservableProperty]
    public partial Visibility MainSideBarToolVisibility { get; set; }

    [RelayCommand]
    private void MainSideBarFrameNavigated(object parameter) {
        if (parameter is Frame frame) {
            if (frame.Content == null) {
                MainSideBarToolVisibility = Visibility.Collapsed;
            }
            else {
                MainSideBarToolVisibility = Visibility.Visible;
            }
        }
    }

    [RelayCommand]
    private void MainSideBarSelectionChanged(object parameter) {
        if (parameter is NavigationView mainSideBar && mainSideBar.SelectedItem != null) {
            SendInvokeFuncMessage(((MainSideBarItem)mainSideBar.SelectedItem).NavigatePage, MainPageMessengerTokenEnum.FlyoutPageNavigateToken);
        }
    }

    [RelayCommand(CanExecute = nameof(MainSideBarFrameCanGoBack))]
    private void BackButton(Frame parameter) {
        if (parameter.CanGoBack) {
            parameter.GoBack();
        }
    }

    [RelayCommand]
    private void CloseButton(Frame parameter) {
        parameter.Content = null;
        MainSideBarToolVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void MainSideBarFlyoutClosed() {
        MainSideBarSelectedItem = null;
    }

    internal void SetCanGoBack() {
        var mainSideBarFrame = SendGetValueMessage<Frame>(MainPageMessengerTokenEnum.MainSideBarFrameToken);
        if (mainSideBarFrame != null) {
            MainSideBarFrameCanGoBack = mainSideBarFrame.Response.CanGoBack;
            if (mainSideBarFrame.Response.Content == null) {
                MainSideBarToolVisibility = Visibility.Collapsed;
            }
            else {
                MainSideBarToolVisibility = Visibility.Visible;
            }
        }
    }

    private void SendInvokeFuncMessage<T>(T value, Enum tokenEnum) {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<T>(value), tokenEnum.ToString());
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }
}
 
