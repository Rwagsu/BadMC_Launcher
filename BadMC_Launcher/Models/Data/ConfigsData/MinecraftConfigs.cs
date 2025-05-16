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
using BadMC_Launcher.Classes.Comparers;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Classes.DataClasses;

namespace BadMC_Launcher.Models.Data.ConfigsData;
internal static class MinecraftConfigs {
    internal static DistinctiveItemBindingList<Account> minecraftAccounts = new() { PropertyName = nameof(MinecraftConfigsService.MinecraftAccounts) };

    internal static DistinctiveItemBindingList<string> javaPaths = new(new JavaPathListComparer()) { PropertyName = nameof(MinecraftConfigsService.JavaPaths) };

    internal static DistinctiveItemBindingList<MinecraftFolderViewItem> minecraftFolders = new() { PropertyName = nameof(MinecraftConfigsService.MinecraftFolders) };

    internal static string? activeJavaPath;

    internal static string? activeMinecraftFolder;

    internal static Account? activeMinecraftAccount;

    internal static bool isAutoJavaEnabled = false;

    internal static bool isFullscreen = false;

    internal static string versionIsolationFilterId = App.GetService<LaunchSettingsService>().VersionIsolationFilters[0].Id;

    internal static bool isAutoMemorySize = true;

    internal static uint maxGameMemory = 1024;

    internal static uint minGameMemory = 512;

    internal static string? launcherName = App.GetService<ResourceLoader>().GetString("MinecraftConfig_MinecraftTitleNameResource");

    internal static BindingList<string> jvmArguments = new();
}
