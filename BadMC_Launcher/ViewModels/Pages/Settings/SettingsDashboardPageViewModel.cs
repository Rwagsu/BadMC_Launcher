using BadMC_Launcher.Models.Data.ViewData;
using BadMC_Launcher.Controls;
using CommunityToolkit.Mvvm.Input;
using BadMC_Launcher.Extensions;

namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class SettingsDashboardPageViewModel : ObservableObject {
    public SettingsDashboardPageViewModel() {
        SideBarItems = SettingsData.settingsSideBarItems;
        FootSideBarItems = SettingsData.settingsSideBarFooterItems;
    }
    [ObservableProperty]
    public partial DistinctiveItemBindingList<SettingsSideBarItem> SideBarItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<SettingsSideBarItem> FootSideBarItems { get; set; }

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
