using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.ViewClasses;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.Services.ViewServices;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Media.Animation;

namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class SettingsDashboardPageViewModel : ObservableObject {
    public SettingsDashboardPageViewModel() {
        SideBarItems = SettingsData.SettingsSideBarItems;
        FootSideBarItems = SettingsData.SettingsSideBarFooterItems;
    }
    [ObservableProperty]
    public partial ObservableCollection<SettingsSideBarItem> SideBarItems { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SettingsSideBarItem> FootSideBarItems { get; set; }

    [ObservableProperty]
    public partial SettingsSideBarItem? SideBarSelectedItem { get; set; }

    [RelayCommand]
    private void SetInitSelectedItem() {
        if (SideBarItems.Count > 0) {
            SideBarSelectedItem = SideBarItems[0];
        }
    }

    [RelayCommand]
    private void NavigateToPage(Frame parameter) {
        if (SideBarSelectedItem != null) {
            parameter.Navigate(SideBarSelectedItem.NavigatePage);
        }
    }
}
