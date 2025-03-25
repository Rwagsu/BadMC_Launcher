using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Controls.Minecraft;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Storage.Pickers;
using WinRT.Interop;
using BadMC_Launcher.Models.Enums;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;
public partial class MinecraftFolderContentDialogViewModel : ObservableObject {
    private MinecraftConfigService minecraftConfigService = App.GetService<MinecraftConfigService>();

    public MinecraftFolderContentDialogViewModel() {
        MinecraftFoldersList = minecraftConfigService.MinecraftFolders;
        MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftConfigService.ActiveMinecraftFolderPath);

        OldRenameMinecraftFolderId = string.Empty;
        NewRenameMinecraftFolderId = string.Empty;
        RenameMinecraftFolderPath = string.Empty;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyRenameCommand))]
    public partial bool CanRename { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<MinecraftFolderEntry> MinecraftFoldersList { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFoldersListSelectedItem { get; set; }

    [ObservableProperty]
    public partial string OldRenameMinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string NewRenameMinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string RenameMinecraftFolderPath { get; set; }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task AddMinecraftFolder(Button parameter) {
        // Create bew folder picker
        FolderPicker folderPicker = new FolderPicker();

        // Get window handle
        var hwnd = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, hwnd);

        // Show folder picker dialog
        StorageFolder folder = await folderPicker.PickSingleFolderAsync();

        if (folder != null) {
            var folderId = "NewFolder";

            if (minecraftConfigService.MinecraftFolders.Any(item => item.MinecraftFolderPath == folder.Path)) {
                //Show tip toast
                return;
            }

            // Add folder to list
            minecraftConfigService.MinecraftFolders.Add(new() {
                MinecraftFolderId = folderId,
                MinecraftFolderPath = folder.Path
            });

            // Refresh list
            MinecraftFoldersList = minecraftConfigService.MinecraftFolders;

            // Set new folder name
            OldRenameMinecraftFolderId = folderId;
            RenameMinecraftFolderPath = folder.Path;
            FlyoutBase.ShowAttachedFlyout(parameter);
        }
    }

    [RelayCommand]
    private void OpenMinecraftFolder() {
        if (RenameMinecraftFolderPath != null) {
            // Open folder
            App.GetService<FileService>().TryOpenFolderFromPath(RenameMinecraftFolderPath);
        }
    }

    [RelayCommand(CanExecute = nameof(CanRename))]
    private void ApplyRename() {
        // Get minecraft folder entry
        var minecraftPathEntry = minecraftConfigService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == RenameMinecraftFolderPath);
        if (minecraftPathEntry != null) {
            // Check if folder with new name already exists
            if (minecraftConfigService.MinecraftFolders.Any(item => item.MinecraftFolderId == NewRenameMinecraftFolderId)) {
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
            MinecraftFoldersList = minecraftConfigService.MinecraftFolders;
            MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftConfigService.ActiveMinecraftFolderPath);

            // Hide Flyout
            HideRenameFlyout();
            return;
        }
        //TODO: Tip Toast
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
            minecraftConfigService.ActiveMinecraftFolderPath = MinecraftFoldersListSelectedItem.MinecraftFolderPath;
            MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftConfigService.ActiveMinecraftFolderPath);
        }
    }

    [RelayCommand]
    private void ViewFolderInLocal(string parameter) {
        if (!App.GetService<FileService>().TryOpenFolderFromPath(parameter)) {
            // Toast Tip
        }
    }

    [RelayCommand]
    private void RenameItem(string parameter) {
        var folderEntryPath = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == parameter)?.MinecraftFolderId;

        if (folderEntryPath != null) {
            SendInvokeFuncMessage(parameter, MinecraftFolderContentDialogMessengerTokenEnum.ShowRenameFlyoutToken);
            // Set new folder name
            OldRenameMinecraftFolderId = folderEntryPath;
            RenameMinecraftFolderPath = parameter;
        }
    }

    [RelayCommand]
    private void DeleteItem(string parameter) {
        var deleteItem = minecraftConfigService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == parameter);
        if (deleteItem != null && minecraftConfigService.MinecraftFolders.Remove(deleteItem)) {
            // Refresh list
            MinecraftFoldersList = minecraftConfigService.MinecraftFolders;
            // TODO: Toast Tip
            return;
        }
        // TODO: Toast Tip(ERROR)
    }

    [RelayCommand]
    private void SetCanRename() {
        // Check if text is not empty and not equal to old folder name
        if (!string.IsNullOrWhiteSpace(NewRenameMinecraftFolderId) && !minecraftConfigService.MinecraftFolders.Any(item => item.MinecraftFolderId == NewRenameMinecraftFolderId)) {
            // Set apply button enabled
            CanRename = true;
            return;
        }

        // Set apply button disabled
        CanRename = false;
    }

    [RelayCommand]
    private void HideRenameFlyout() => SendInvokeFuncMessage(string.Empty, MinecraftFolderContentDialogMessengerTokenEnum.HideRenameFlyoutToken);

    private ValueChangedMessage<T> SendInvokeFuncMessage<T>(T value, Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new ValueChangedMessage<T>(value), tokenEnum.ToString());
    }
}
