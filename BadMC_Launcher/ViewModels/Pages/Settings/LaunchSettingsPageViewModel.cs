using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Enums;
using BadMC_Launcher.Enums.MessengerTokenEnum;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using BadMC_Launcher.Servicess.Settings;
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
    private MinecraftConfigService service = App.GetService<MinecraftConfigService>();

    public LaunchSettingsPageViewModel() {
        mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MainPageMessengerTokenEnum.XamlRootToken).Response;
        Java = service.ActiveJavaPath;
        MinecraftFolder = service.MinecraftFolders.First(item => item.MinecraftFolderPath == service.ActiveMinecraftFolderPath);
        IsAutoMemorySize = service.IsAutoMemorySize;
    }

    [ObservableProperty]
    public partial MinecraftFolderEntry? MinecraftFolder { get; set; }

    [ObservableProperty]
    public partial JavaEntry? Java { get; set; }

    [ObservableProperty]
    public partial bool IsAutoMemorySize { get; set; }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task MinecraftFolderSettingsCardClicked() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task JavaPathSettingsCardClicked() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand]
    private void IsAutoMemorySizeSwitchToggled() {
        service.IsAutoMemorySize = IsAutoMemorySize;
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }
}
