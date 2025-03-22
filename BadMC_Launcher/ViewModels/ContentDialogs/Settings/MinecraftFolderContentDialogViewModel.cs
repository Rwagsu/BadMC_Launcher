using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Enums;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services;
using BadMC_Launcher.Services.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;
public partial class MinecraftFolderContentDialogViewModel : ObservableObject {
    private MinecraftConfigService minecraftCOnfigService = App.GetService<MinecraftConfigService>();
    private MinecraftConfigService MinecraftService = App.GetService<MinecraftConfigService>();

    public MinecraftFolderContentDialogViewModel() {
        MinecraftFoldersList = minecraftCOnfigService.MinecraftFolders.ToObservableCollection();
        MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == MinecraftService.ActiveMinecraftFolderPath);

        OldRenameMinecraftFolderId = string.Empty;
        NewRenameMinecraftFolderId = string.Empty;
        RenameMinecraftFolderPath = string.Empty;

        CanAddMinecraftFolder = true;
        RenameFlyoutCanApply = false;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddMinecraftFolderCommand))]
    public partial bool CanAddMinecraftFolder { get; set; }

    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyRenameCommand))]
    public partial bool RenameFlyoutCanApply { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderEntry> MinecraftFoldersList { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFoldersListSelectedItem { get; set; }

    [ObservableProperty]
    public partial string OldRenameMinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string NewRenameMinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string RenameMinecraftFolderPath { get; set; }

    [RelayCommand(CanExecute = nameof(CanAddMinecraftFolder), FlowExceptionsToTaskScheduler = true)]
    private async Task AddMinecraftFolder(Button parameter) {
        CanAddMinecraftFolder = false;

        // Create bew folder picker
        FolderPicker folderPicker = new FolderPicker();

        // Get window handle
        var hwnd = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, hwnd);

        // Show folder picker dialog
        StorageFolder folder = await folderPicker.PickSingleFolderAsync();

        if (folder != null) {

            // Set init folder name
            var folderId = "NewFolder";

            // Add folder to list
            MinecraftService.MinecraftFolders.Add(new() {
                MinecraftFolderId = folderId,
                MinecraftFolderPath = folder.Path
            });

            // Refresh list
            MinecraftFoldersList = minecraftCOnfigService.MinecraftFolders.ToObservableCollection();

            // Set new folder name
            OldRenameMinecraftFolderId = folderId;
            RenameMinecraftFolderPath = folder.Path;
            FlyoutBase.ShowAttachedFlyout(parameter);
        }

        CanAddMinecraftFolder = true;
    }

    [RelayCommand]
    private void OpenMinecraftFolder() {
        if (RenameMinecraftFolderPath != null) {
            // Open folder
            App.GetService<FileService>().TryOpenFolderFromPath(RenameMinecraftFolderPath);
        }
    }

    [RelayCommand]
    private void SetRenameFlyoutCanApply(TextBox parameter) {
        // Check if text is not empty and not equal to old folder name
        if (!string.IsNullOrWhiteSpace(parameter.Text) && parameter.Text != OldRenameMinecraftFolderId) {
            RenameFlyoutCanApply = true;
            return;
        }
        RenameFlyoutCanApply = false;
    }

    [RelayCommand(CanExecute = nameof(RenameFlyoutCanApply))]
    private void ApplyRename() {
        // Get minecraft folder entry
        var minecraftPathEntry = MinecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == RenameMinecraftFolderPath);
        if (minecraftPathEntry != null) {
            // Check if folder with new name already exists
            if (MinecraftService.MinecraftFolders.Any(item => item.MinecraftFolderId == NewRenameMinecraftFolderId)) {
                //Show tip toast
                //TODO: Tip Toast
                return;
            }

            if (string.IsNullOrWhiteSpace(NewRenameMinecraftFolderId) && NewRenameMinecraftFolderId == OldRenameMinecraftFolderId) {
                //TODO tip toast
                return;
            }

            // Rename folder
            minecraftPathEntry.MinecraftFolderId = NewRenameMinecraftFolderId;

            // Refresh list
            MinecraftFoldersList = minecraftCOnfigService.MinecraftFolders.ToObservableCollection();

            // Hide flyout
            var message = SendGetValueMessage<Flyout>(MinecraftFolderContentDialogMessengerTokenEnum.RenameFlyoutToken);
            message.Response.Hide();
            return;
        }
        //TODO: Tip Toast
    }

    [RelayCommand]
    private void HideFlyout(Button parameter) {
        //Get Flyout
        var message = SendGetValueMessage<Flyout>(MinecraftFolderContentDialogMessengerTokenEnum.RenameFlyoutToken);

        //Hide flyout
        message.Response.Hide();
    }

    [RelayCommand]
    private void DeleteRenameFlyoutData() {
        // Clear data
        RenameMinecraftFolderPath = string.Empty;
        OldRenameMinecraftFolderId = string.Empty;
        NewRenameMinecraftFolderId = string.Empty;
    }

    [RelayCommand]
    private void SetActiveMinecraftFolder() {
        if (MinecraftFoldersListSelectedItem != null) {
            MinecraftService.ActiveMinecraftFolderPath = MinecraftFoldersListSelectedItem.MinecraftFolderPath;
        }
    }

    [RelayCommand]
    private void ViewFolderInLocal(string parameter) {
        if (!App.GetService<FileService>().TryOpenFolderFromPath(parameter)) {
            // Toast Tip
        }
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }
}
