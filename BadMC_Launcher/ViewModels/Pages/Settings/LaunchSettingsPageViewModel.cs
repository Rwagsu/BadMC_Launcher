using System.ComponentModel;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;
using Serilog;

namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class LaunchSettingsPageViewModel : ObservableObject {
    private readonly CancellationTokenSource cancelLoopToken = new();
    private readonly XamlRoot? mainPageXamlRoot;
    private readonly MinecraftConfigsService minecraftService = App.GetService<MinecraftConfigsService>();
    private readonly ResourceLoader sourceService = App.GetService<ResourceLoader>();
    private bool isRangeSelectorDragStarted = false;
    private MinecraftFolderViewItem? minecraftFolder;
    private JavaEntry? java;
   
    public LaunchSettingsPageViewModel() {
        mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MainPageMessengerTokenEnum.XamlRootToken).Response;
        minecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;

        MinecraftFolderId = minecraftFolder != null ? minecraftFolder.MinecraftFolderId : sourceService.GetString("Global_NullMinecraftFolderId");
        MinecraftFolderPath = minecraftFolder != null ? minecraftFolder.MinecraftFolderPath : sourceService.GetString("Global_NullMinecraftFolderPath");

        JavaId = sourceService.GetString("Global_NullJavaId");
        JavaPath = sourceService.GetString("Global_NullJavaPath");

        IsAutoGameMemorySize = minecraftService.IsAutoMemorySize;
        MinGameMemory = minecraftService.MinGameMemory;
        MaxGameMemory = minecraftService.MaxGameMemory;

        RefreshMemory(cancelLoopToken.Token);

        GetJavaInfo();
    }

    // Minecraft folder settings
    [ObservableProperty]
    public partial string MinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string MinecraftFolderPath { get; set; }

    // Java settings
    [ObservableProperty]
    public partial string JavaId { get; set; }

    [ObservableProperty]
    public partial string JavaPath { get; set; }

    // Memory settings
    [ObservableProperty]
    public partial bool IsAutoGameMemorySize { get; set; }

    [ObservableProperty]
    public partial uint MaxMemoryView { get; set; }

    [ObservableProperty]
    public partial uint UsedMemoryView { get; set; }

    [ObservableProperty]
    public partial uint GameMemoryView { get; set; }

    [ObservableProperty]
    public partial uint MinGameMemory { get; set; }

    [ObservableProperty]
    public partial uint MaxGameMemory { get; set; }

    [RelayCommand]
    private void CancelLoop() {
        if (cancelLoopToken.Token.CanBeCanceled) {
            cancelLoopToken.Cancel();
        }
        
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task ShowMinecraftFolderManagerDialog() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task ShowJavaManagerDialog() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<JavaContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand]
    private void DisableNumberBoxEvent() {
        isRangeSelectorDragStarted = true;
    }

    [RelayCommand]
    private void EnableNumberBoxEvent() {
        isRangeSelectorDragStarted = false;
    }

    [RelayCommand]
    private void SetGameMemory() {
        if (isRangeSelectorDragStarted) {
            return;
        }
        // Set min memory
        minecraftService.MinGameMemory = MinGameMemory;

        // Set max memory
        minecraftService.MaxGameMemory = MaxGameMemory;
    }

    partial void OnIsAutoGameMemorySizeChanged(bool value) {
        minecraftService.IsAutoMemorySize = IsAutoGameMemorySize;

        if (value) {
            GameMemoryView = MaxMemoryView.GetAutoGameMemoryMb(UsedMemoryView);
        }
        else {
            GameMemoryView = MaxGameMemory;
        }
    }

    private async void GetJavaInfo() {
        java = await JavaUtil.GetJavaInfoAsync(minecraftService.ActiveJavaPath);

        // Set Java information
        JavaId = minecraftService.IsAutoJavaEnabled ?
             $"{sourceService.GetString("Global_AutoJavaPath")}" :
             java != null ? $"{java.JavaType} {java.JavaVersion}" : sourceService.GetString("Global_NullJavaId");
        JavaPath = java != null ? java.JavaPath : sourceService.GetString("Global_NullJavaPath");
    }

    private async void RefreshMemory(CancellationToken cancellationToken) {
        try {
            while (true) {
                AppParameters.SystemInfo.RefreshMemoryStatus();

                MaxMemoryView = AppParameters.SystemInfo.MemoryStatus.TotalPhysical.BytesToMb();
                UsedMemoryView = (AppParameters.SystemInfo.MemoryStatus.TotalPhysical - AppParameters.SystemInfo.MemoryStatus.AvailablePhysical).BytesToMb();

                if (IsAutoGameMemorySize) {
                    GameMemoryView = MaxMemoryView.GetAutoGameMemoryMb(UsedMemoryView);
                }
                else {
                    GameMemoryView = MaxGameMemory;
                }

                // Wait 1 second
                await Task.Delay(1000, cancellationToken);
            }
        }
        catch(TaskCanceledException ex) {
            Log.Information($"RefreshMemory method is Exited: {ex.Source}");
        }
    }
    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }


    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(MinecraftConfigsService.ActiveJavaPath):
            case nameof(MinecraftConfigsService.IsAutoJavaEnabled):
                if (minecraftService.ActiveJavaPath != JavaId || JavaId != sourceService.GetString("Global_NullJavaId")) {
                    GetJavaInfo();
                }
                break;
            case nameof(MinecraftConfigsService.ActiveMinecraftFolderPath):
                if (minecraftService.ActiveMinecraftFolderPath != MinecraftFolderPath) {
                    minecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

                    MinecraftFolderId = minecraftFolder != null ? minecraftFolder.MinecraftFolderId : sourceService.GetString("Global_NullMinecraftFolderId");
                    MinecraftFolderPath = minecraftFolder != null ? minecraftFolder.MinecraftFolderPath : sourceService.GetString("Global_NullMinecraftFolderPath");
                }
                break;
            case nameof(MinecraftConfigsService.MinGameMemory):
                if (minecraftService.MinGameMemory != MinGameMemory) {
                    MinGameMemory = minecraftService.MinGameMemory;
                }
                break;
            case nameof(MinecraftConfigsService.MaxGameMemory):
                if (minecraftService.MaxGameMemory != MaxGameMemory) {
                    MaxGameMemory = minecraftService.MaxGameMemory;
                }
                break;
        }
    }

    partial void OnMaxGameMemoryChanged(uint value)  {
        
    }
}
