using System.Collections.ObjectModel;
using System.ComponentModel;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.MainSearch;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Views.Pages;
using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Uno.UI.RemoteControl;

namespace BadMC_Launcher.ViewModels.Pages;

public partial class MainPageViewModel : ObservableObject {

    public MainPageViewModel() {
        //Init Property
        WindowName = App.GetService<ThemeSettingService>().WindowName;

        MainSideBarToolVisibility = Visibility.Collapsed;
        MainSideBarItems = MainSideBarData.MainSideBarItems;
        MainSideBarFooterItems = MainSideBarData.MainSideBarFooterItems;

        SearchFilterItems = MainSideBarData.MainMenuSharchFilterItems;
        SearchItems = new();
        SearchFilterSelectedItems = new();
        SearchText = string.Empty;

        App.GetService<ThemeSettingService>().SetBackground((brush) => {
            AppBackground = brush;
        });
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PageGoBackCommand))]
    public partial bool MainSideBarFrameCanGoBack { get; set; }

    [ObservableProperty]
    public partial string WindowName { get; set; }
    
    [ObservableProperty]
    public partial Brush? AppBackground { get; set; }

    //MainSideBar Items
    [ObservableProperty]
    public partial DistinctiveItemBindingList<MainSideBarItem> MainSideBarItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<MainSideBarItem> MainSideBarFooterItems { get; set; }

    [ObservableProperty]
    public partial MainSideBarItem? MainSideBarSelectedItem { get; set; }

    [ObservableProperty]
    public partial Visibility MainSideBarToolVisibility { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<IMainMenuSharchFilterItem> SearchFilterItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<IMainMenuSharchFilterItem> SearchFilterSelectedItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<MainMenuSearchResultItem> SearchItems { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; }

    [RelayCommand]
    private void ChangeToolVisibilityAndSelectedItem(Frame parameter) {
        if (parameter.Content == null) {
            MainSideBarToolVisibility = Visibility.Collapsed;
        }
        else {
            MainSideBarToolVisibility = Visibility.Visible;
        }

        // Set CanGoBack
        MainSideBarFrameCanGoBack = parameter.CanGoBack;

        var list = new List<MainSideBarItem>();
        list.AddRange(MainSideBarItems);
        list.AddRange(MainSideBarFooterItems);

        // Set SelectedItem
        MainSideBarSelectedItem = list.FirstOrDefault(item => item.NavigatePage == parameter.Content?.GetType());
    }   

    [RelayCommand]
    private void NavigateToPage(NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem != null && args.SelectedItem != MainSideBarSelectedItem) {
            SendInvokeFuncMessage(((MainSideBarItem)args.SelectedItem).NavigatePage, MainPageMessengerTokenEnum.PageNavigateToken);
        }
    }

    [RelayCommand(CanExecute = nameof(MainSideBarFrameCanGoBack))]
    private void PageGoBack(Frame parameter) {
        if (parameter.CanGoBack) {
            parameter.GoBack();
        }
    }

    [RelayCommand]
    private void ClosePage(Frame parameter) {
        parameter.Content = null;
        MainSideBarToolVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void DeselectItem() {
        MainSideBarSelectedItem = null;
    }

    [RelayCommand]
    private void SearchItem(AutoSuggestBoxTextChangedEventArgs e) {
        if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
            var searchItems = new DistinctiveItemBindingList<MainMenuSearchResultItem>();
            foreach (var item in SearchFilterSelectedItems) {
                foreach (var searchMainMenuSearchResultItem in item.Search(SearchText)) {
                    searchItems.Add(searchMainMenuSearchResultItem);
                }
            }
            SearchItems = searchItems;
        }
    }

    [RelayCommand]
    private void SearchItemNavigateToPage(AutoSuggestBoxSuggestionChosenEventArgs e) {
        if (e.SelectedItem is MainMenuSearchResultItem selectedItem) {
            selectedItem.Navigate.Invoke();
        }
    }

    [RelayCommand]
    private void OpenSearchFilter(AutoSuggestBox parameter) {
        FlyoutBase.ShowAttachedFlyout(parameter);
    }

    [RelayCommand]
    private void ChangeSearchFilter(IMainMenuSharchFilterItem[] parameter) {
        SearchFilterSelectedItems.Clear();
        SearchFilterSelectedItems.AddRange(parameter.ToObservableCollection());
    }

    private void SendInvokeFuncMessage<T>(T value, Enum tokenEnum) {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<T>(value), tokenEnum.ToString());
    }
}
 
