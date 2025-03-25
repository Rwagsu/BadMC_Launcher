using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using System.Text.Json.Serialization;
using System.ComponentModel;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using BadMC_Launcher.Models.Datas;
using BadMC_Launcher.Models.Datas.Mappings;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls.Minecraft;

namespace BadMC_Launcher.Services.Settings;
public class MinecraftConfigService : ConfigClass {
    internal bool isSyncEnabled = false;

    public MinecraftConfigService() {
        //Triggers an event when a property is changed
        MinecraftConfig.minecraftAccounts.ListChanged += OnListChanged;
        MinecraftConfig.javaPaths.ListChanged += OnListChanged;
        MinecraftConfig.minecraftFolders.ListChanged += OnListChanged;
        MinecraftConfig.jvmArguments.ListChanged += OnListChanged;
    }

    public DistinctiveItemBindingList<Account> MinecraftAccounts {
        get => MinecraftConfig.minecraftAccounts;
        set {
            MinecraftConfig.minecraftAccounts.Clear();
            MinecraftConfig.minecraftAccounts.AddRange(value);

            // Write to Json
            SyncSettingSet();
        }
    }
    public DistinctiveItemBindingList<string> JavaPaths {
        get => MinecraftConfig.javaPaths;
        set {
            MinecraftConfig.javaPaths.Clear();
            MinecraftConfig.javaPaths.AddRange(value);

            // Write to Json
            SyncSettingSet();
        }
    }
    public DistinctiveItemBindingList<MinecraftFolderEntry> MinecraftFolders {
        get => MinecraftConfig.minecraftFolders;
        set {
            MinecraftConfig.minecraftFolders.Clear();
            MinecraftConfig.minecraftFolders.AddRange(value);

            // Write to Json
            SyncSettingSet();
        }
    }

    public JavaEntry? ActiveJavaPath {
        get => MinecraftConfig.activeJavaPath;
        set {
            MinecraftConfig.activeJavaPath = value;

            // Trigger Event
            OnPropertyChanged(nameof(ActiveJavaPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? ActiveMinecraftFolderPath {
        get => MinecraftConfig.activeMinecraftFolder;
        set {
            MinecraftConfig.activeMinecraftFolder = value;

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftFolderPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public Account? ActiveMinecraftAccount {
        get => MinecraftConfig.activeMinecraftAccount;
        set {
            MinecraftConfig.activeMinecraftAccount = value;

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftAccount));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsFullscreen {
        get => MinecraftConfig.isFullscreen;
        set {
            MinecraftConfig.isFullscreen = value;

            // Trigger Event
            OnPropertyChanged(nameof(JavaPaths));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsEnableIndependencyCore {
        get => MinecraftConfig.isEnableIndependencyCore;
        set {
            MinecraftConfig.isEnableIndependencyCore = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsEnableIndependencyCore));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsAutoMemorySize {
        get => MinecraftConfig.isAutoMemorySize;
        set {
            MinecraftConfig.isAutoMemorySize = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsAutoMemorySize));

            //Write to Json
            SyncSettingSet();
        }
    }

    public int MinMemorySize {
        get => MinecraftConfig.minMemorySize;
        set {
            MinecraftConfig.minMemorySize = value;

            // Trigger Event
            OnPropertyChanged(nameof(MinMemorySize));

            //Write to Json
            SyncSettingSet();
        }
    }

    public int MaxMemorySize {
        get => MinecraftConfig.maxMemorySize;
        set {
            MinecraftConfig.maxMemorySize = value;

            // Trigger Event
            OnPropertyChanged(nameof(MaxMemorySize));

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? LauncherName {
        get => MinecraftConfig.launcherName;
        set {
            MinecraftConfig.launcherName = value;

            // Trigger Event
            OnPropertyChanged(nameof(LauncherName));

            //Write to Json
            SyncSettingSet();
        }
    }

    public BindingList<string> JvmArguments {
        get => MinecraftConfig.jvmArguments;
        set {
            MinecraftConfig.jvmArguments.Clear();
            MinecraftConfig.jvmArguments.AddRange(value);

            // Write to Json
            SyncSettingSet();
        }
    }

    private void OnListChanged(object? sender, ListChangedEventArgs e) {
        if (!SyncSettingSet()) {
            //TODO: Dialog
        }
    }

    public override bool SyncSettingGet() {
        if (App.GetService<FileService>().ReadConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigServiceContext.Default.MinecraftConfigService, out var jsonClass, UpdateMapping.MinecraftConfig) && jsonClass != null) {
            //TODO: 解蜜
            MinecraftConfig.minecraftAccounts = jsonClass.MinecraftAccounts;
            MinecraftConfig.javaPaths = jsonClass.JavaPaths;
            MinecraftConfig.minecraftFolders = jsonClass.MinecraftFolders;
            MinecraftConfig.activeJavaPath = jsonClass.ActiveJavaPath;
            MinecraftConfig.activeMinecraftFolder = jsonClass.ActiveMinecraftFolderPath;
            MinecraftConfig.activeMinecraftAccount = jsonClass.ActiveMinecraftAccount;
            MinecraftConfig.isFullscreen = jsonClass.IsFullscreen;
            MinecraftConfig.isEnableIndependencyCore = jsonClass.IsEnableIndependencyCore;
            MinecraftConfig.isAutoMemorySize = jsonClass.IsAutoMemorySize;
            MinecraftConfig.minMemorySize = jsonClass.MinMemorySize;
            MinecraftConfig.maxMemorySize = jsonClass.MaxMemorySize;
            MinecraftConfig.launcherName = jsonClass.LauncherName;
            MinecraftConfig.jvmArguments = jsonClass.JvmArguments;
            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (isSyncEnabled == false) {
            return false;
        }
        MinecraftConfigService classValue = this;
        //TODO: 加蜜
        return App.GetService<FileService>().WriteConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigServiceContext.Default.MinecraftConfigService, classValue);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MinecraftConfigService))]
internal partial class MinecraftConfigServiceContext : JsonSerializerContext;
