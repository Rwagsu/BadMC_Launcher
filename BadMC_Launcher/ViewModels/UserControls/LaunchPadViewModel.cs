using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Servicess.Settings;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Extensions;

namespace BadMC_Launcher.ViewModels.UserControls;
public partial class LaunchPadViewModel : ObservableObject {
    MinecraftConfigService MinecraftService = App.GetService<MinecraftConfigService>();
    MinecraftFolderEntry? minecraftPathEntry;
    ObservableCollection<MinecraftItem> minecraftList = new ObservableCollection<MinecraftItem>();

    public LaunchPadViewModel() {
        //Check Active Minecraft Path
        if (MinecraftService.ActiveMinecraftFolderPath != null) {

            //Get Minecraft Entry
            minecraftPathEntry = MinecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == MinecraftService.ActiveMinecraftFolderPath);
            if (minecraftPathEntry != null) {

                //Get Minecraft Items
                foreach (var minecraftEntry in minecraftPathEntry.GetMinecrafts()) {
                    var item = minecraftPathEntry.GetMinecraftItem(minecraftEntry);
                    if (item != null)
                    minecraftList.Add(item);
                }
                MinecraftList = minecraftList;


                if (minecraftPathEntry.ActiveMinecraftEntryId != null) {
                    var minecraftItem = minecraftPathEntry.GetMinecraftItem(minecraftPathEntry.ActiveMinecraftEntryId);
                    if (minecraftItem != null) {
                        MinecraftListSelectedItem = MinecraftList.FirstOrDefault(item => item.MinecraftId == minecraftItem.MinecraftId);
                    }
                }
                MinecraftFolderListSelectedItem = minecraftPathEntry;

            }
        }
        MinecraftFolderList = new ObservableCollection<MinecraftFolderEntry>(MinecraftService.MinecraftFolders);
        SetLaunchButtonEntry();
    }

    //LaunchButton Property
    [ObservableProperty]
    public partial string? GameEntryName { get; set; }

    [ObservableProperty]
    public partial BitmapImage? GameEntryImage { get; set; }

    //MinecraftsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftItem>? MinecraftList { get; set; }

    //MinecraftFolderPathsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderEntry>? MinecraftFolderList { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFolderListSelectedItem { get; set; }

    [ObservableProperty]
    public partial MinecraftItem? MinecraftListSelectedItem { get; set; }

    [RelayCommand]
    public void LaunchButton() {

    }

    //Refresh MinecraftList
    [RelayCommand]
    public void RefreshMinecraftList() {
        //Get Configs From Json File
        MinecraftService.SyncSettingGet();
        minecraftPathEntry = MinecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == MinecraftService.ActiveMinecraftFolderPath);
        if (MinecraftService.ActiveMinecraftFolderPath != null && minecraftPathEntry != null) {
            minecraftPathEntry.GetMinecrafts();
            MinecraftList = minecraftList;
            SetLaunchButtonEntry();
        }
    }

    [RelayCommand]
    public void ViewLoaclMinecraftFolder() {
        if (MinecraftService.ActiveMinecraftFolderPath != null) {
            try {
                using (Process.Start(new ProcessStartInfo(MinecraftService.ActiveMinecraftFolderPath) {
                    UseShellExecute = true,
                    Verb = "open"
                })) {

                }
            }
            catch (Exception ex) {
                switch (ex) {
                    case Win32Exception:
                        //TODO: Dialog
                        break;
                    case FileNotFoundException:
                        //TODO: Dialog
                        break;
                    default:
                        throw;
                }
            }
        }
    }

    [RelayCommand]
    public void MinecraftListSelected(object parameter) {
        if (parameter is ListView listView) {
            var item = (MinecraftItem)listView.SelectedItem;
            if (item != null && MinecraftService.ActiveMinecraftFolderPath != null && minecraftPathEntry != null) {
                minecraftPathEntry.ActiveMinecraftEntryId = item.MinecraftId;
                SetLaunchButtonEntry();
            }
        }
    }

    [RelayCommand]
    public void MinecraftFoldersListSelected(object parameter) {
        if (parameter is ComboBox comboBox) {
            var item = (MinecraftFolderEntry)comboBox.SelectedItem;
            MinecraftService.ActiveMinecraftFolderPath = item.MinecraftFolderPath;
            minecraftPathEntry = MinecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == MinecraftService.ActiveMinecraftFolderPath);
            if (MinecraftService.ActiveMinecraftFolderPath != null && minecraftPathEntry != null) {
                MinecraftList = (ObservableCollection<MinecraftItem>)minecraftPathEntry.GetMinecraftItems();
                if (minecraftPathEntry.ActiveMinecraftEntryId != null) {
                    MinecraftListSelectedItem = minecraftPathEntry.GetMinecraftItem(minecraftPathEntry.ActiveMinecraftEntryId);
                }
            }
            else {
                MinecraftList = null;
            }
            SetLaunchButtonEntry();
        }
    }


    [RelayCommand]
    public void MinecraftEmptyListHyperLink() {
        if (MinecraftList != null && !MinecraftList.Any()) {
            return;
        }

    }

    public void SetLaunchButtonEntry() {
        if (minecraftPathEntry != null && minecraftPathEntry.ActiveMinecraftEntryId != null) {
            var minecraftItem = minecraftPathEntry.GetMinecraftItem(minecraftPathEntry.ActiveMinecraftEntryId);
            if (minecraftItem != null) {
                GameEntryName = minecraftItem.MinecraftId;
                GameEntryImage = minecraftItem.MinecraftImage;
                return;
            }
        }
        GameEntryName = App.GetService<ResourceLoader>().GetString("LaunchPad_LaunchButtonTagDefaultResource");
        GameEntryImage = new BitmapImage() { UriSource = new Uri(@"ms-appx:///Assets/Icons/MinecraftIcons/Drowned.png") };
    }
}
