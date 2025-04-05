using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using MinecraftLaunch.Base.Models.Game;
using Newtonsoft.Json.Linq;
using Uno.Extensions.Specialized;
using BadMC_Launcher.Models.Enums;

namespace BadMC_Launcher.ViewModels.UserControls;
public partial class LaunchPadViewModel : ObservableObject {
    MinecraftConfigService minecraftService = App.GetService<MinecraftConfigService>();

    public LaunchPadViewModel() {
        MinecraftFolderEntryList = minecraftService.MinecraftFolders.ToObservableCollection();
        MinecraftEntryList = new();

        //Check active Minecraft path
        if (minecraftService.ActiveMinecraftFolderPath != null) {
            // Set MinecraftFolderEntryListSelectedIndex
            MinecraftFolderEntryListSelectedIndex = MinecraftFolderEntryList.GetIndex(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

            // Set Minecraft entries
            if (MinecraftFolderEntryList.TryElementAt(MinecraftFolderEntryListSelectedIndex, out var selectedItem) && selectedItem != null) {
                // Get minecraftItems
                MinecraftEntryList = new(selectedItem.GetMinecraftItems().ToList());

                // Set ActiveMinecraftEntry
                MinecraftEntryListSelectedIndex = MinecraftEntryList.GetIndex(item => item.MinecraftId == selectedItem.ActiveMinecraftEntryId);
            }
        }

        LaunchButtonMinecraftId = MinecraftEntryList.ElementAtOrDefault(MinecraftEntryListSelectedIndex)?.MinecraftId ?? App.GetService<ResourceLoader>().GetString("LaunchPad_LaunchButtonTagDefaultResource");
        LaunchButtonMinecraftIcon = MinecraftEntryList.ElementAtOrDefault(MinecraftEntryListSelectedIndex)?.MinecraftImage ?? new() { UriSource = new(@"ms-appx:///Assets/Icons/MinecraftIcons/drowned.png") };

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;

        IsMinecraftEntryListEmpty = !MinecraftEntryList.Any();
    }

    //LaunchButton Property
    [ObservableProperty]
    public partial string? LaunchButtonMinecraftId { get; set; }

    [ObservableProperty]
    public partial BitmapImage? LaunchButtonMinecraftIcon { get; set; }

    //MinecraftsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftViewItem> MinecraftEntryList { get; set; }

    [ObservableProperty]
    public partial bool IsMinecraftEntryListEmpty { get; set; }

    //MinecraftFolderPathsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderViewItem> MinecraftFolderEntryList { get; set; }

    [ObservableProperty]
    public partial int MinecraftFolderEntryListSelectedIndex { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ViewLoaclMinecraftFolderCommand), nameof(LaunchMinecraftJavaCommand))]
    public partial int MinecraftEntryListSelectedIndex { get; set; }

    [RelayCommand(CanExecute = nameof(SetIsNotActiveMinecraftEntryEmpty))]
    public void LaunchMinecraftJava() {
        
    }

    // Invoke Refresh Command
    [RelayCommand]
    private void InvokeRefreshMinecraftEntryList() {
        RefreshMinecraftEntryList();
    }

    // Refresh MinecraftEntryList
    [RelayCommand]
    private void RefreshMinecraftEntryList(RefreshRequestedEventArgs args) {
        using (args.GetDeferral()) {
            RefreshMinecraftEntryList();
        }
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task OpenMinecraftFoldersManager() {
        var mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MainPageMessengerTokenEnum.XamlRootToken).Response;

        if (mainPageXamlRoot != null) {

            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(SetIsNotActiveMinecraftEntryEmpty))]
    private void ViewLoaclMinecraftFolder() {
        if (MinecraftFolderEntryList.TryElementAt(MinecraftFolderEntryListSelectedIndex, out var selectedItem) && selectedItem != null) {
            App.GetService<FileService>().TryOpenFolderFromPath(selectedItem.MinecraftFolderPath);
        }
    }

    [RelayCommand]
    private void ChangeActiveMinecraftFolder() {
        if (MinecraftFolderEntryList.TryElementAt(MinecraftFolderEntryListSelectedIndex, out var selectedItem) && selectedItem != null) {
            minecraftService.ActiveMinecraftFolderPath = selectedItem.MinecraftFolderPath;

            // Get active minecraft path entry
            if (minecraftService.ActiveMinecraftFolderPath != null) {
                // Set MinecraftEntryList
                MinecraftEntryList = new(selectedItem.GetMinecraftItems().ToList());

                // Set ActiveMinecraftEntry
                MinecraftEntryListSelectedIndex = MinecraftEntryList.GetIndex(item => item.MinecraftId == selectedItem.ActiveMinecraftEntryId);
            }
            else {
                // Init MinecraftEntryList
                MinecraftEntryList = new();
            }
        }
    }

    partial void OnMinecraftEntryListSelectedIndexChanged(int value) {
        if (value >= 0) {
            // Get ActiveMinecraftEntry
            var minecraftFolderEntry = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

            if (minecraftFolderEntry != null) {
                minecraftFolderEntry.ActiveMinecraftEntryId = MinecraftEntryList.ElementAtOrDefault(value)?.MinecraftId;
            }
        }
        LaunchButtonMinecraftId = MinecraftEntryList.ElementAtOrDefault(MinecraftEntryListSelectedIndex)?.MinecraftId ?? App.GetService<ResourceLoader>().GetString("LaunchPad_LaunchButtonTagDefaultResource");
        LaunchButtonMinecraftIcon = MinecraftEntryList.ElementAtOrDefault(MinecraftEntryListSelectedIndex)?.MinecraftImage ?? new() { UriSource = new(@"ms-appx:///Assets/Icons/MinecraftIcons/drowned.png")};
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs args) {
        var selectedItem = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

        // Update Property
        switch (args.PropertyName) {
            case nameof(MinecraftConfigService.ActiveMinecraftFolderPath):
                // Set MinecraftFolderEntries SelectedIndex
                MinecraftFolderEntryListSelectedIndex = MinecraftFolderEntryList.GetIndex(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath); 

                // Update MinecraftEntryList
                MinecraftEntryList.Clear();
                MinecraftEntryList.AddRange(selectedItem?.GetMinecraftItems());

                MinecraftEntryListSelectedIndex = MinecraftEntryList.GetIndex(item => item.MinecraftId == selectedItem?.ActiveMinecraftEntryId);
                break;
            case nameof(MinecraftConfigService.MinecraftFolders):
                // Update MinecraftFolderEntryList
                MinecraftFolderEntryList = minecraftService.MinecraftFolders.ToObservableCollection();
                
                // Get ActiveMinecraftEntry
                MinecraftFolderEntryListSelectedIndex = MinecraftFolderEntryList.GetIndex(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

                // Update MinecraftEntryList
                MinecraftEntryList.Clear();
                MinecraftEntryList.AddRange(selectedItem?.GetMinecraftItems());

                MinecraftEntryListSelectedIndex = MinecraftEntryList.GetIndex(item => item.MinecraftId == selectedItem?.ActiveMinecraftEntryId);
                break;
        }

        // Set whether to display the empty list prompt
        IsMinecraftEntryListEmpty = !MinecraftEntryList.Any();
    }

    //Set CanExecute for ViewLoaclMinecraftFolderCommand
    private bool SetIsNotActiveMinecraftEntryEmpty() {
        if (MinecraftEntryList.Any()) {
            return MinecraftEntryListSelectedIndex >= 0;
        }
        return false;
    }

    private void RefreshMinecraftEntryList() {
        if (MinecraftFolderEntryList.TryElementAt(MinecraftFolderEntryListSelectedIndex, out var selectedItem) && selectedItem != null) {
            // Get minecraftItems
            MinecraftEntryList = new(selectedItem.GetMinecraftItems().ToList());

            // Set ActiveMinecraftEntry
            MinecraftEntryListSelectedIndex = MinecraftEntryList.GetIndex(item => item.MinecraftId == selectedItem.ActiveMinecraftEntryId);

            return;
        }

        // Clear MinecraftEntryList
        MinecraftEntryList.Clear();
    }

    partial void OnMinecraftEntryListChanged(ObservableCollection<MinecraftViewItem> value) {
        IsMinecraftEntryListEmpty = !MinecraftEntryList.Any();
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }

    partial void OnIsMinecraftEntryListEmptyChanged(bool value) {
        
    }
}
