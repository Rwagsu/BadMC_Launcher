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
        MinecraftFolderEntryList = minecraftService.MinecraftFolders;
        MinecraftEntryList = new();

        //Check active Minecraft path
        if (minecraftService.ActiveMinecraftFolderPath != null) {
            //Get active Minecraft path entry
            var activeMinecraftPathEntry = MinecraftFolderEntryList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

            if (activeMinecraftPathEntry != null) {

                // Set MinecraftFolderEntryListSelectedItem
                MinecraftFolderEntryListSelectedItem = activeMinecraftPathEntry;
            }
        }
    }

    //LaunchButton Property
    [ObservableProperty]
    public partial string? LaunchButtonMinecraftId { get; set; }

    //MinecraftsList
    [ObservableProperty]
    public partial DistinctiveItemBindingList<MinecraftEntryItem> MinecraftEntryList { get; set; }

    //MinecraftFolderPathsList
    [ObservableProperty]
    public partial DistinctiveItemBindingList<MinecraftFolderEntry> MinecraftFolderEntryList { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFolderEntryListSelectedItem { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ViewLoaclMinecraftFolderCommand), nameof(LaunchMinecraftJavaCommand))]
    public partial MinecraftEntryItem? MinecraftEntryListSelectedItem { get; set; }

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
        if (MinecraftFolderEntryListSelectedItem != null) {
            App.GetService<FileService>().TryOpenFolderFromPath(MinecraftFolderEntryListSelectedItem.MinecraftFolderPath);
        }
    }

    partial void OnMinecraftFolderEntryListSelectedItemChanged(MinecraftFolderEntry? value) {
        if (value != null && MinecraftFolderEntryListSelectedItem != null) {
            minecraftService.ActiveMinecraftFolderPath = value.MinecraftFolderPath;
        }

        // Get active minecraft path entry
        if (value != null && minecraftService.ActiveMinecraftFolderPath != null && MinecraftFolderEntryListSelectedItem != null) {
            // Set MinecraftEntryList
            MinecraftEntryList = new(value.GetMinecraftItems().ToList());

            // Set ActiveMinecraftEntry
            MinecraftEntryListSelectedItem = MinecraftEntryList.FirstOrDefault(item => item.MinecraftId == value.ActiveMinecraftEntryId);
        }

        // Init MinecraftEntryList if null
        if (MinecraftEntryList == null) {
            MinecraftEntryList = new();
        }
    }

    partial void OnMinecraftEntryListSelectedItemChanged(MinecraftEntryItem? value) {
        if (value != null && MinecraftFolderEntryListSelectedItem != null) {
            MinecraftFolderEntryListSelectedItem.ActiveMinecraftEntryId = value.MinecraftId;
            LaunchButtonMinecraftId = MinecraftEntryListSelectedItem?.MinecraftId ?? App.GetService<ResourceLoader>().GetString("LaunchPad_LaunchButtonTagDefaultResource");
        }
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        // Update Property
        switch (e.PropertyName) {
            case nameof(MinecraftConfigService.ActiveMinecraftFolderPath):
                if (sender is MinecraftConfigService activeMinecraftFolderPathService) { MinecraftFolderEntryListSelectedItem = MinecraftFolderEntryList.FirstOrDefault(item => item.MinecraftFolderPath == activeMinecraftFolderPathService.ActiveMinecraftFolderPath); }
                break;
            case nameof(MinecraftConfigService.MinecraftFolders):
                if (sender is MinecraftConfigService minecraftFoldersService) { MinecraftFolderEntryList = minecraftFoldersService.MinecraftFolders; }
                break;
        }
    }

    //Set CanExecute for ViewLoaclMinecraftFolderCommand
    private bool SetIsNotActiveMinecraftEntryEmpty() => MinecraftEntryListSelectedItem != null;

    // TODO: 拆
    private void RefreshMinecraftEntryList() {
        //Get Configs From Json File
        minecraftService.SyncSettingGet();
        if (MinecraftFolderEntryListSelectedItem != null) {
            // Init list
            var minecraftEntries = new ObservableCollection<MinecraftEntryItem>();

            // Get minecraftItems
            MinecraftEntryList = new(MinecraftFolderEntryListSelectedItem.GetMinecraftItems().ToList());

            // Set ActiveMinecraftEntry
            MinecraftEntryListSelectedItem = MinecraftEntryList.FirstOrDefault(item => item.MinecraftId == MinecraftFolderEntryListSelectedItem.ActiveMinecraftEntryId);
        }
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }
}
