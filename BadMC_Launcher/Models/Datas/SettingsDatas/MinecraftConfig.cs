using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using Microsoft.Windows.ApplicationModel.Resources;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using BadMC_Launcher.Controls.Minecraft;
using System.ComponentModel;
using BadMC_Launcher.Services.Settings;

namespace BadMC_Launcher.Models.Datas.SettingsDatas;
internal static class MinecraftConfig {
    internal static DistinctiveItemBindingList<Account> minecraftAccounts = new() { PropertyName = nameof(MinecraftConfigService.MinecraftAccounts) };

    internal static DistinctiveItemBindingList<string> javaPaths = new() { PropertyName = nameof(MinecraftConfigService.JavaPaths) };

    internal static DistinctiveItemBindingList<MinecraftFolderEntry> minecraftFolders = new() { PropertyName = nameof(MinecraftConfigService.MinecraftFolders) };

    internal static string? activeJavaPath;

    internal static string? activeMinecraftFolder;

    internal static Account? activeMinecraftAccount;

    internal static bool isFullscreen = false;

    internal static bool isEnableIndependencyCore = false;

    internal static bool isAutoMemorySize = true;

    internal static int minMemorySize = 512;

    internal static int maxMemorySize = 1024;

    internal static string? launcherName = App.GetService<ResourceLoader>().GetString("MinecraftConfig_MinecraftTitleNameResource");

    internal static BindingList<string> jvmArguments = new();
}
