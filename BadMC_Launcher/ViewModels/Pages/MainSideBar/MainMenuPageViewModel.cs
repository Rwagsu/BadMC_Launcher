using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Classes.ViewClasses;
using BadMC_Launcher.Classes.ViewClasses.MainSearch;
using BadMC_Launcher.Enums;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.Services.ViewServices;
using BadMC_Launcher.Views.Pages.MainSideBar;
using BadMC_Launcher.Views.Pages.Settings;
using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.Controls;
using Microsoft.Windows.ApplicationModel.Resources;

namespace BadMC_Launcher.ViewModels.Pages.MainSideBar;
public partial class MainMenuPageViewModel : ObservableObject {
    public MainMenuPageViewModel() {
        MainMenuItems = MainMenuData.MainMenuItems;
        SearchFilterItems = MainMenuData.MainMenuSharchFilterItems;
        SearchItems = new();
        SearchFilterSelectedItems = new();
        SearchFilterRealTimeToggleIsOn = true;
        SearchText = string.Empty;
    }

    [ObservableProperty]
    public partial ObservableCollection<IMainMenuSharchFilterItem> SearchFilterItems { get; set; }
    
    [ObservableProperty]
    public partial List<IMainMenuSharchFilterItem> SearchFilterSelectedItems { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<MainMenuSearchResultItem> SearchItems { get; set; }
    
    [ObservableProperty]
    public partial string SearchText { get; set; }
    
    [ObservableProperty]
    public partial bool SearchFilterRealTimeToggleIsOn { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<MainMenuItem> MainMenuItems { get; set; }

    //TODO: 该写Blog了，还能这样的啊？？？Σ(っ °Д °;)っ
    [RelayCommand]
    private void SearchTextChanged_SearchItem(AutoSuggestBoxQuerySubmittedEventArgs e) {
        MainMenuSearch(e.QueryText);
    }
    
    [RelayCommand]
    private void SearchButtonClicked_SearchItem(AutoSuggestBoxTextChangedEventArgs e) {
        if (SearchFilterRealTimeToggleIsOn && e.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
            MainMenuSearch(SearchText);
        }
    }

    [RelayCommand]
    private void SearchItemNavigateToPage(AutoSuggestBoxSuggestionChosenEventArgs e) {
        if (e.SelectedItem is MainMenuSearchResultItem selectedItem) {
            selectedItem.Navigate.Invoke();
        }
    }
    
    [RelayCommand]
    private void AddSearchFilter(TokenView parameter) {
         parameter.SelectedItems.ForEach(item => {
             if (item is IMainMenuSharchFilterItem mainMenuSharchFilterItem) {
                 SearchFilterSelectedItems.Add(mainMenuSharchFilterItem);
             }
         });
    }
    
    [RelayCommand]
    private void NavigateToSettingsPage() {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Type>(typeof(SettingsDashboardPage)), MainPageMessengerTokenEnum.PageNavigateToken.ToString());
    }

    private void MainMenuSearch(string queryText) {
        var searchItems = new ObservableDataList<MainMenuSearchResultItem>();
        foreach (var item in SearchFilterSelectedItems) {
            foreach (var searchMainMenuSearchResultItem in item.Search(queryText)) {
                searchItems.Add(searchMainMenuSearchResultItem);
            }
        }
        SearchItems = searchItems;
    }
}
