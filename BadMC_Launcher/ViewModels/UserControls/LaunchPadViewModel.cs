using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Classes.ViewClasses.Minecraft;
using BadMC_Launcher.Enums;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using MinecraftLaunch.Base.Models.Game;
using Newtonsoft.Json.Linq;
using Uno.Extensions.Specialized;

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

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;
    }

    //Set CanExecute for ViewLoaclMinecraftFolderCommand
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ViewLoaclMinecraftFolderCommand), nameof(LaunchMinecraftJavaCommand))]
    public partial bool IsNotActiveMinecraftEntryEmpty { get; set; }

    //LaunchButton Property
    [ObservableProperty]
    public partial string? GameEntryName { get; set; }

    [ObservableProperty]
    public partial BitmapImage? GameEntryImage { get; set; }

    //MinecraftsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftEntryItem> MinecraftEntryList { get; set; }

    //MinecraftFolderPathsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderEntry> MinecraftFolderEntryList { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFolderEntryListSelectedItem { get; set; }

    [ObservableProperty]
    public partial MinecraftEntryItem? MinecraftEntryListSelectedItem { get; set; }

    [RelayCommand]
    public void LaunchMinecraftJava() {
        
    }

    //Refresh MinecraftEntryList
    [RelayCommand]
    private void RefreshMinecraftEntryList() {
        //Get Configs From Json File
        minecraftService.SyncSettingGet();
        if (MinecraftFolderEntryListSelectedItem != null) {
            // Init list
            var minecraftEntries = new ObservableCollection<MinecraftEntryItem>();

            // Get minecraftItems
            MinecraftEntryList = MinecraftFolderEntryListSelectedItem.GetMinecraftItems().ToObservableCollection();

            IsNotActiveMinecraftEntryEmpty = MinecraftEntryListSelectedItem != null;
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

    [RelayCommand(CanExecute = nameof(IsNotActiveMinecraftEntryEmpty))]
    private void ViewLoaclMinecraftFolder() {
        if (MinecraftFolderEntryListSelectedItem != null) {
            App.GetService<FileService>().TryOpenFolderFromPath(MinecraftFolderEntryListSelectedItem.MinecraftFolderPath);
        }
    }

    private void SetLaunchButtonEntry() {
        if (MinecraftFolderEntryListSelectedItem != null && MinecraftFolderEntryListSelectedItem.ActiveMinecraftEntryId != null) {
            // Get MinecraftItem
            var minecraftItem = MinecraftFolderEntryListSelectedItem.GetMinecraftItem(MinecraftFolderEntryListSelectedItem.ActiveMinecraftEntryId);

            // Set GameEntryName and GameEntryImage
            if (minecraftItem != null) {
                GameEntryName = minecraftItem.MinecraftId;
                GameEntryImage = minecraftItem.MinecraftImage;
                return;
            }
        }

        // If MinecraftItem is null, set default values
        GameEntryName = App.GetService<ResourceLoader>().GetString("LaunchPad_LaunchButtonTagDefaultResource");
        GameEntryImage = new BitmapImage() { UriSource = new Uri(@"ms-appx:///Assets/Icons/MinecraftIcons/Drowned.png") };
    }

    partial void OnMinecraftFolderEntryListSelectedItemChanged(MinecraftFolderEntry? value) {
        if (value != null && MinecraftFolderEntryListSelectedItem != null) {
            minecraftService.ActiveMinecraftFolderPath = value.MinecraftFolderPath;
        }

        // Get active minecraft path entry
        if (value != null && minecraftService.ActiveMinecraftFolderPath != null && MinecraftFolderEntryListSelectedItem != null) {
            // Set MinecraftEntryList
            MinecraftEntryList = value.GetMinecraftItems().ToObservableCollection();

            // If ActiveMinecraftEntryId is not null, set ActiveMinecraftEntry
            if (value.ActiveMinecraftEntryId != null) {
                MinecraftEntryListSelectedItem = MinecraftEntryList.FirstOrDefault(item => item.MinecraftId == value.ActiveMinecraftEntryId);
            }
        }

        // Init MinecraftEntryList if null
        if (MinecraftEntryList == null) {
            MinecraftEntryList = new();
        }

        IsNotActiveMinecraftEntryEmpty = MinecraftEntryListSelectedItem != null;

        // Update LaunchBotton Entry
        SetLaunchButtonEntry();
    }

    partial void OnMinecraftEntryListSelectedItemChanged(MinecraftEntryItem? value) {
        if (value != null && MinecraftFolderEntryListSelectedItem != null) {
            MinecraftFolderEntryListSelectedItem.ActiveMinecraftEntryId = value.MinecraftId;
        }

        IsNotActiveMinecraftEntryEmpty = value != null;

        // Update LaunchBotton Entry
        SetLaunchButtonEntry();
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

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }
}
