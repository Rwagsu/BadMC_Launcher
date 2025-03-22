using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Enums;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using BadMC_Launcher.Views.Pages.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;

namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class LaunchSettingsPageViewModel : ObservableObject {
    private readonly XamlRoot? mainPageXamlRoot;
    private MinecraftConfigService minecraftService = App.GetService<MinecraftConfigService>();

    public LaunchSettingsPageViewModel() {
        mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MainPageMessengerTokenEnum.XamlRootToken).Response;
        Java = minecraftService.ActiveJavaPath;
        MinecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);
        IsAutoMemorySize = minecraftService.IsAutoMemorySize;

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;
    }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFolder { get; set; }

    [ObservableProperty]
    public partial JavaEntry? Java { get; set; }

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
    private async Task ShowJavaPathManagerDialog() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand]
    private void SetIsAutoMemorySize() {
        minecraftService.IsAutoMemorySize = IsAutoMemorySize;
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(minecraftService.ActiveMinecraftFolderPath)) {
            MinecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);
        }
    }
}
