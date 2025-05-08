using System.ComponentModel;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.Mappings;
using BadMC_Launcher.Models.Data.SettingsData;
using BadMC_Launcher.Models.Enums;
using MinecraftLaunch.Base.Models.Authentication;

namespace BadMC_Launcher.Services.Settings;
public class MinecraftConfigService : ConfigClass {
    internal bool IsSyncEnabled = false;

    public MinecraftConfigService() {
        //Triggers an event when a property is changed
        MinecraftConfig.minecraftAccounts.ListChanged += OnListChanged<Account>;
        MinecraftConfig.javaPaths.ListChanged += OnListChanged<string>;
        MinecraftConfig.minecraftFolders.ListChanged += OnListChanged<MinecraftFolderViewItem>;
        MinecraftConfig.jvmArguments.ListChanged += OnListChanged<string>;
    }

    public DistinctiveItemBindingList<Account> MinecraftAccounts {
        get => MinecraftConfig.minecraftAccounts;
        set {
            if (!MinecraftConfig.minecraftAccounts.SequenceEqual(value)) {
                MinecraftConfig.minecraftAccounts.Clear();
                MinecraftConfig.minecraftAccounts.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }
    public DistinctiveItemBindingList<string> JavaPaths {
        get {
            // Check if the Java folder exists
            foreach (var item in MinecraftConfig.javaPaths.ToList()) {
                if (!Path.Exists(item)) {
                    MinecraftConfig.javaPaths.Remove(item);
                    // TODO: Tip Toast
                }
            }
            
            return MinecraftConfig.javaPaths;
        }
        set {
            if (!MinecraftConfig.javaPaths.SequenceEqual(value)) {
                MinecraftConfig.javaPaths.Clear();
                MinecraftConfig.javaPaths.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }
    public DistinctiveItemBindingList<MinecraftFolderViewItem> MinecraftFolders {
        get {
            // Check if the Minecraft folder exists
            foreach (var item in MinecraftConfig.minecraftFolders.ToList()) {
                if (!Path.Exists(item.MinecraftFolderPath)) {
                    MinecraftConfig.minecraftFolders.Remove(item);
                    // TODO: Tip Toast
                }
            }

            return MinecraftConfig.minecraftFolders;
        }
        set {
            if (!MinecraftConfig.minecraftFolders.SequenceEqual(value)) {
                MinecraftConfig.minecraftFolders.Clear();
                MinecraftConfig.minecraftFolders.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string? ActiveJavaPath {
        get => MinecraftConfig.activeJavaPath;
        set {
            // Check folder is existing
            if (!Path.Exists(value)) {
                MinecraftConfig.activeJavaPath = string.Empty;
                // TODO: Tip Toast
            }
            else {
                MinecraftConfig.activeJavaPath = value;
            }

            // Trigger Event
            OnPropertyChanged(nameof(ActiveJavaPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? ActiveMinecraftFolderPath {
        get => MinecraftConfig.activeMinecraftFolder;
        set {
            // Check folder is existing
            if (!Path.Exists(value)) {
                MinecraftConfig.activeMinecraftFolder = string.Empty;
                // TODO: Tip Toast
            }
            else {
                MinecraftConfig.activeMinecraftFolder = value;
            }

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftFolderPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public Account? ActiveMinecraftAccount {
        get => MinecraftConfig.activeMinecraftAccount;
        set {
            if (MinecraftConfig.activeMinecraftAccount != value) {
                MinecraftConfig.activeMinecraftAccount = value;

                // Trigger Event
                OnPropertyChanged(nameof(ActiveMinecraftAccount));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoJavaEnabled {
        get => MinecraftConfig.isAutoJavaEnabled;
        set {
            if (MinecraftConfig.isAutoJavaEnabled != value) {
                MinecraftConfig.isAutoJavaEnabled = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoJavaEnabled));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsFullscreen {
        get => MinecraftConfig.isFullscreen;
        set {
            if (MinecraftConfig.isFullscreen != value) {
                MinecraftConfig.isFullscreen = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsFullscreen));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public IndependencyCoreEnum IndependencyCore {
        get => MinecraftConfig.independencyCore;
        set {
            if (MinecraftConfig.independencyCore != value) {
                MinecraftConfig.independencyCore = value;

                // Trigger Event
                OnPropertyChanged(nameof(IndependencyCore));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoMemorySize {
        get => MinecraftConfig.isAutoMemorySize;
        set {
            if (MinecraftConfig.isAutoMemorySize != value) {
                MinecraftConfig.isAutoMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoMemorySize));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MaxGameMemory {
        get => MinecraftConfig.maxGameMemory;
        set {
            if (MinecraftConfig.maxGameMemory != value) {
                // Check max game memory
                AppParameters.SystemInfo.RefreshMemoryStatus();
                if (value == 0 || value > AppParameters.SystemInfo.MemoryStatus.TotalPhysical.BytesToMb() || value <= MinecraftConfig.minGameMemory) {
                    // TODO: Toast Tips
                    OnPropertyChanged(nameof(MaxGameMemory));
                    return;
                }

                MinecraftConfig.maxGameMemory = value;

                // Trigger Event
                OnPropertyChanged(nameof(MaxGameMemory));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MinGameMemory {
        get => MinecraftConfig.minGameMemory;
        set {
            if (MinecraftConfig.minGameMemory != value) {
                // Check min game memory
                AppParameters.SystemInfo.RefreshMemoryStatus();
                if (value == 0 || value > AppParameters.SystemInfo.MemoryStatus.TotalPhysical.BytesToMb() || value >= MinecraftConfig.maxGameMemory) {
                    // TODO: Toast Tips
                    OnPropertyChanged(nameof(MinGameMemory));
                    return;
                }

                MinecraftConfig.minGameMemory = value;

                // Trigger Event
                OnPropertyChanged(nameof(MinGameMemory));

                //Write to Json
                SyncSettingSet();

            }
        }
    }

    public string? LauncherName {
        get => MinecraftConfig.launcherName;
        set {
            if (MinecraftConfig.launcherName != value) {
                MinecraftConfig.launcherName = value;

                // Trigger Event
                OnPropertyChanged(nameof(LauncherName));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public BindingList<string> JvmArguments {
        get => MinecraftConfig.jvmArguments;
        set {
            if (!MinecraftConfig.jvmArguments.SequenceEqual(value)) {
                MinecraftConfig.jvmArguments.RaiseListChangedEvents = false;

                MinecraftConfig.jvmArguments.Clear();
                MinecraftConfig.jvmArguments.AddRange(value);

                MinecraftConfig.jvmArguments.RaiseListChangedEvents = true;

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    private void OnListChanged<T>(object? sender, ListChangedEventArgs e) {
        if (sender is DistinctiveItemBindingList<T> senderList && !string.IsNullOrWhiteSpace(senderList.PropertyName)) {
            switch (senderList.PropertyName) {
                case nameof(MinecraftFolders):
                    if (MinecraftFolders.All(item => item.MinecraftFolderPath != ActiveMinecraftFolderPath) && !string.IsNullOrWhiteSpace(ActiveMinecraftFolderPath)) {
                        ActiveMinecraftFolderPath = string.Empty;
                    }
                    break;
                case nameof(JavaPaths):
                    if (JavaPaths.All(item => item != ActiveJavaPath) && !string.IsNullOrWhiteSpace(ActiveJavaPath)) {
                        ActiveJavaPath = string.Empty;
                    }
                    break;
                case nameof(MinecraftAccounts):
                    if (MinecraftAccounts.All(item => item.Uuid != ActiveMinecraftAccount?.Uuid) && ActiveMinecraftAccount != null) {
                        ActiveMinecraftAccount = null;
                    }
                    break;
            }

            // Notify list changed
            OnPropertyChanged(senderList.PropertyName);
        }

        if (!SyncSettingSet()) {
            //TODO: Dialog
        }
    }

    public override bool SyncSettingGet() {
        if (App.GetService<FileService>().TryReadConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigServiceContext.Default.MinecraftConfigService, out var jsonClass, UpdateMapping.minecraftConfigPropertyNameMapping, UpdateMapping.minecraftConfigPropertyTypeMapping) && jsonClass != null) {
            //TODO: 解蜜
            MinecraftConfig.activeJavaPath = jsonClass.ActiveJavaPath;
            MinecraftConfig.activeMinecraftFolder = jsonClass.ActiveMinecraftFolderPath;
            //MinecraftConfig.activeMinecraftAccount = jsonClass.ActiveMinecraftAccount;
            MinecraftConfig.isAutoJavaEnabled = jsonClass.IsAutoJavaEnabled;
            MinecraftConfig.isFullscreen = jsonClass.IsFullscreen;
            MinecraftConfig.independencyCore = jsonClass.IndependencyCore;
            MinecraftConfig.isAutoMemorySize = jsonClass.IsAutoMemorySize;
            MinecraftConfig.minGameMemory = jsonClass.MinGameMemory;
            MinecraftConfig.maxGameMemory = jsonClass.MaxGameMemory;
            MinecraftConfig.launcherName = jsonClass.LauncherName;

            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (IsSyncEnabled == false) {
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
