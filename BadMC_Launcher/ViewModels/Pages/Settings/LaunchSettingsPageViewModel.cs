using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.ViewModels.ContentDialogs.Settings;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using BadMC_Launcher.Views.Pages.Settings;
using BadMC_Launcher.Views.UserControls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;


namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class LaunchSettingsPageViewModel : ObservableObject {
    private readonly XamlRoot? mainPageXamlRoot;
    private MinecraftConfigService minecraftService = App.GetService<MinecraftConfigService>();
    private ResourceLoader sourceService = App.GetService<ResourceLoader>();
    private MinecraftFolderViewItem? minecraftFolder;
    private JavaEntry? java;

    public LaunchSettingsPageViewModel() {
        mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MainPageMessengerTokenEnum.XamlRootToken).Response;
        minecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);
        IsAutoMemorySize = minecraftService.IsAutoMemorySize;

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;

        MinecraftFolderId = minecraftFolder != null ? minecraftFolder.MinecraftFolderId : sourceService.GetString("Global_NullMinecraftFolderId");
        MinecraftFolderPath = minecraftFolder != null ? minecraftFolder.MinecraftFolderPath : sourceService.GetString("Global_NullMinecraftFolderPath");

        JavaId = sourceService.GetString("Global_NullJavaId");
        JavaPath = sourceService.GetString("Global_NullJavaPath");

        GetJavaInfo();
    }

    // Minecraft folder settings
    [ObservableProperty]
    public partial string MinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string MinecraftFolderPath { get; set; }

    [ObservableProperty]
    public partial string JavaId { get; set; }

    [ObservableProperty]
    public partial string JavaPath { get; set; }

    [ObservableProperty]
    public partial bool IsAutoMemorySize { get; set; }

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
    private void SetIsAutoMemorySize() {
        minecraftService.IsAutoMemorySize = IsAutoMemorySize;
    }

    private async void GetJavaInfo() {
        java = await JavaUtil.GetJavaInfoAsync(minecraftService.ActiveJavaPath);

        // Set Java information
        JavaId = minecraftService.IsAutoJavaEnabled ?
             $"{sourceService.GetString("Global_AutoJavaPath")}" :
             ( java != null ? $"{java.JavaType} {java.JavaVersion}" : sourceService.GetString("Global_NullJavaId") );
        JavaPath = java != null ? java.JavaPath : sourceService.GetString("Global_NullJavaPath");
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }


    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(minecraftService.ActiveJavaPath):
            case nameof(minecraftService.IsAutoJavaEnabled):
                GetJavaInfo();
                break;
            case nameof(minecraftService.MinecraftFolders):
            case nameof(minecraftService.ActiveMinecraftFolderPath):
                minecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

                MinecraftFolderId = minecraftFolder != null ? minecraftFolder.MinecraftFolderId : sourceService.GetString("Global_NullMinecraftFolderId");
                MinecraftFolderPath = minecraftFolder != null ? minecraftFolder.MinecraftFolderPath : sourceService.GetString("Global_NullMinecraftFolderPath");
                break;
        }
    }
}
