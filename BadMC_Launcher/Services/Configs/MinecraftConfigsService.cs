using System.ComponentModel;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.Mappings;
using BadMC_Launcher.Models.Data.SettingsData;
using BadMC_Launcher.Models.Enums;
using MinecraftLaunch.Base.Models.Authentication;

namespace BadMC_Launcher.Services.Configs;
public class MinecraftConfigsService : ConfigClass {
    internal bool IsSyncEnabled = false;

    public MinecraftConfigsService() {
        //Triggers an event when a property is changed
        MinecraftConfigs.minecraftAccounts.ListChanged += OnListChanged<Account>;
        MinecraftConfigs.javaPaths.ListChanged += OnListChanged<string>;
        MinecraftConfigs.minecraftFolders.ListChanged += OnListChanged<MinecraftFolderViewItem>;
        MinecraftConfigs.jvmArguments.ListChanged += OnListChanged<string>;
    }

    public DistinctiveItemBindingList<Account> MinecraftAccounts {
        get => MinecraftConfigs.minecraftAccounts;
        set {
            if (!MinecraftConfigs.minecraftAccounts.SequenceEqual(value)) {
                MinecraftConfigs.minecraftAccounts.Clear();
                MinecraftConfigs.minecraftAccounts.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }
    public DistinctiveItemBindingList<string> JavaPaths {
        get {
            // Check if the Java folder exists
            foreach (var item in MinecraftConfigs.javaPaths.ToList()) {
                if (!Path.Exists(item)) {
                    MinecraftConfigs.javaPaths.Remove(item);
                    // TODO: Tip Toast
                }
            }
            
            return MinecraftConfigs.javaPaths;
        }
        set {
            if (!MinecraftConfigs.javaPaths.SequenceEqual(value)) {
                MinecraftConfigs.javaPaths.Clear();
                MinecraftConfigs.javaPaths.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }
    public DistinctiveItemBindingList<MinecraftFolderViewItem> MinecraftFolders {
        get {
            // Check if the Minecraft folder exists
            foreach (var item in MinecraftConfigs.minecraftFolders.ToList()) {
                if (!Path.Exists(item.MinecraftFolderPath)) {
                    MinecraftConfigs.minecraftFolders.Remove(item);
                    // TODO: Tip Toast
                }
            }

            return MinecraftConfigs.minecraftFolders;
        }
        set {
            if (!MinecraftConfigs.minecraftFolders.SequenceEqual(value)) {
                MinecraftConfigs.minecraftFolders.Clear();
                MinecraftConfigs.minecraftFolders.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string? ActiveJavaPath {
        get => MinecraftConfigs.activeJavaPath;
        set {
            // Check folder is existing
            if (!Path.Exists(value)) {
                MinecraftConfigs.activeJavaPath = string.Empty;
                // TODO: Tip Toast
            }
            else {
                MinecraftConfigs.activeJavaPath = value;
            }

            // Trigger Event
            OnPropertyChanged(nameof(ActiveJavaPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? ActiveMinecraftFolderPath {
        get => MinecraftConfigs.activeMinecraftFolder;
        set {
            // Check folder is existing
            if (!Path.Exists(value)) {
                MinecraftConfigs.activeMinecraftFolder = string.Empty;
                // TODO: Tip Toast
            }
            else {
                MinecraftConfigs.activeMinecraftFolder = value;
            }

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftFolderPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public Account? ActiveMinecraftAccount {
        get => MinecraftConfigs.activeMinecraftAccount;
        set {
            if (MinecraftConfigs.activeMinecraftAccount != value) {
                MinecraftConfigs.activeMinecraftAccount = value;

                // Trigger Event
                OnPropertyChanged(nameof(ActiveMinecraftAccount));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoJavaEnabled {
        get => MinecraftConfigs.isAutoJavaEnabled;
        set {
            if (MinecraftConfigs.isAutoJavaEnabled != value) {
                MinecraftConfigs.isAutoJavaEnabled = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoJavaEnabled));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsFullscreen {
        get => MinecraftConfigs.isFullscreen;
        set {
            if (MinecraftConfigs.isFullscreen != value) {
                MinecraftConfigs.isFullscreen = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsFullscreen));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public VersionIsolationEnum VersionIsolation {
        get => MinecraftConfigs.VersionIsolation;
        set {
            if (MinecraftConfigs.VersionIsolation != value) {
                MinecraftConfigs.VersionIsolation = value;

                // Trigger Event
                OnPropertyChanged(nameof(VersionIsolation));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoMemorySize {
        get => MinecraftConfigs.isAutoMemorySize;
        set {
            if (MinecraftConfigs.isAutoMemorySize != value) {
                MinecraftConfigs.isAutoMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoMemorySize));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MaxGameMemory {
        get => MinecraftConfigs.maxGameMemory;
        set {
            if (MinecraftConfigs.maxGameMemory != value) {
                // Check max game memory
                AppParameters.SystemInfo.RefreshMemoryStatus();
                if (value == 0 || value > AppParameters.SystemInfo.MemoryStatus.TotalPhysical.BytesToMb() || value <= MinecraftConfigs.minGameMemory) {
                    // TODO: Toast Tips
                    OnPropertyChanged(nameof(MaxGameMemory));
                    return;
                }

                MinecraftConfigs.maxGameMemory = value;

                // Trigger Event
                OnPropertyChanged(nameof(MaxGameMemory));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MinGameMemory {
        get => MinecraftConfigs.minGameMemory;
        set {
            if (MinecraftConfigs.minGameMemory != value) {
                // Check min game memory
                AppParameters.SystemInfo.RefreshMemoryStatus();
                if (value == 0 || value > AppParameters.SystemInfo.MemoryStatus.TotalPhysical.BytesToMb() || value >= MinecraftConfigs.maxGameMemory) {
                    // TODO: Toast Tips
                    OnPropertyChanged(nameof(MinGameMemory));
                    return;
                }

                MinecraftConfigs.minGameMemory = value;

                // Trigger Event
                OnPropertyChanged(nameof(MinGameMemory));

                //Write to Json
                SyncSettingSet();

            }
        }
    }

    public string? LauncherName {
        get => MinecraftConfigs.launcherName;
        set {
            if (MinecraftConfigs.launcherName != value) {
                MinecraftConfigs.launcherName = value;

                // Trigger Event
                OnPropertyChanged(nameof(LauncherName));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public BindingList<string> JvmArguments {
        get => MinecraftConfigs.jvmArguments;
        set {
            if (!MinecraftConfigs.jvmArguments.SequenceEqual(value)) {
                MinecraftConfigs.jvmArguments.RaiseListChangedEvents = false;

                MinecraftConfigs.jvmArguments.Clear();
                MinecraftConfigs.jvmArguments.AddRange(value);

                MinecraftConfigs.jvmArguments.RaiseListChangedEvents = true;

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
        if (App.GetService<FileService>().TryReadConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigsServiceContext.Default.MinecraftConfigsService, out var jsonClass, UpdateMapping.minecraftConfigPropertyNameMapping, UpdateMapping.minecraftConfigPropertyTypeMapping) && jsonClass != null) {
            //TODO: 解蜜
            MinecraftConfigs.activeJavaPath = jsonClass.ActiveJavaPath;
            MinecraftConfigs.activeMinecraftFolder = jsonClass.ActiveMinecraftFolderPath;
            //MinecraftConfig.activeMinecraftAccount = jsonClass.ActiveMinecraftAccount;
            MinecraftConfigs.isAutoJavaEnabled = jsonClass.IsAutoJavaEnabled;
            MinecraftConfigs.isFullscreen = jsonClass.IsFullscreen;
            MinecraftConfigs.VersionIsolation = jsonClass.VersionIsolation;
            MinecraftConfigs.isAutoMemorySize = jsonClass.IsAutoMemorySize;
            MinecraftConfigs.minGameMemory = jsonClass.MinGameMemory;
            MinecraftConfigs.maxGameMemory = jsonClass.MaxGameMemory;
            MinecraftConfigs.launcherName = jsonClass.LauncherName;

            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (IsSyncEnabled == false) {
            return false;
        }
        MinecraftConfigsService classValue = this;
        //TODO: 加蜜
        return App.GetService<FileService>().WriteConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigsServiceContext.Default.MinecraftConfigsService, classValue);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MinecraftConfigsService))]
internal partial class MinecraftConfigsServiceContext : JsonSerializerContext;
