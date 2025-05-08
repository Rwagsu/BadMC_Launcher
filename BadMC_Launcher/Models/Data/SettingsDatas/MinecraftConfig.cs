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

namespace BadMC_Launcher.Models.Data.SettingsData;
internal static class MinecraftConfig {
    internal static DistinctiveItemBindingList<Account> minecraftAccounts = new() { PropertyName = nameof(MinecraftConfigService.MinecraftAccounts) };

    internal static DistinctiveItemBindingList<string> javaPaths = new(new JavaPathListComparer()) { PropertyName = nameof(MinecraftConfigService.JavaPaths) };

    internal static DistinctiveItemBindingList<MinecraftFolderViewItem> minecraftFolders = new() { PropertyName = nameof(MinecraftConfigService.MinecraftFolders) };

    internal static string? activeJavaPath;

    internal static string? activeMinecraftFolder;

    internal static Account? activeMinecraftAccount;

    internal static bool isAutoJavaEnabled = false;

    internal static bool isFullscreen = false;

    internal static IndependencyCoreEnum independencyCore = IndependencyCoreEnum.ModLoader;

    internal static bool isAutoMemorySize = true;

    internal static uint maxGameMemory = 1024;

    internal static uint minGameMemory = 512;

    internal static string? launcherName = App.GetService<ResourceLoader>().GetString("MinecraftConfig_MinecraftTitleNameResource");

    internal static BindingList<string> jvmArguments = new();
}
