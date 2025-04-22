using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Models.Datas;
using BadMC_Launcher.Models.Datas.Mappings;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using Uno.Extensions;
using Uno.Extensions.Specialized;

namespace BadMC_Launcher.Services.Settings;
public class MinecraftConfigService : ConfigClass {
    internal bool isSyncEnabled = false;

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
            MinecraftConfig.minecraftAccounts.Clear();
            MinecraftConfig.minecraftAccounts.MargeItems(value);

            // Write to Json
            SyncSettingSet();
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
            MinecraftConfig.javaPaths.Clear();
            MinecraftConfig.javaPaths.MargeItems(value);

            // Write to Json
            SyncSettingSet();
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
            MinecraftConfig.minecraftFolders.Clear();
            MinecraftConfig.minecraftFolders.MargeItems(value);

            // Write to Json
            SyncSettingSet();
        }
    }

    public string? ActiveJavaPath {
        get => MinecraftConfig.activeJavaPath;
        set {
            // CHeck folder is exists
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
            // CHeck folder is exists
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
            MinecraftConfig.activeMinecraftAccount = value;

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftAccount));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsAutoJavaEnabled {
        get => MinecraftConfig.isAutoJavaEnabled;
        set {
            MinecraftConfig.isAutoJavaEnabled = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsAutoJavaEnabled));

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
            MinecraftConfig.jvmArguments.RaiseListChangedEvents = false;

            MinecraftConfig.jvmArguments.Clear();
            MinecraftConfig.jvmArguments.AddRange(value);

            MinecraftConfig.jvmArguments.RaiseListChangedEvents = true;

            // Write to Json
            SyncSettingSet();
        }
    }

    private void OnListChanged<T>(object? sender, ListChangedEventArgs e) {
        if (sender is DistinctiveItemBindingList<T> senderList && !string.IsNullOrWhiteSpace(senderList.PropertyName)) {
            switch (senderList.PropertyName) {
                case nameof(MinecraftFolders):
                    if (!MinecraftFolders.Any(item => item.MinecraftFolderPath == ActiveMinecraftFolderPath) && !string.IsNullOrWhiteSpace(ActiveMinecraftFolderPath)) {
                        ActiveMinecraftFolderPath = string.Empty;
                    }
                    break;
                case nameof(JavaPaths):
                    if (!JavaPaths.Any(item => item == ActiveJavaPath) && !string.IsNullOrWhiteSpace(ActiveJavaPath)) {
                        ActiveJavaPath = string.Empty;
                    }
                    break;
                case nameof(MinecraftAccounts):
                    if (!MinecraftAccounts.Any(item => item.Uuid == ActiveMinecraftAccount?.Uuid) && ActiveMinecraftAccount != null) {
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
        if (App.GetService<FileService>().ReadConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigServiceContext.Default.MinecraftConfigService, out var jsonClass, UpdateMapping.MinecraftConfig) && jsonClass != null) {
            //TODO: 解蜜
            MinecraftConfig.minecraftAccounts = jsonClass.MinecraftAccounts;
            MinecraftConfig.javaPaths = jsonClass.JavaPaths;
            MinecraftConfig.minecraftFolders = jsonClass.MinecraftFolders;
            MinecraftConfig.activeJavaPath = jsonClass.ActiveJavaPath;
            MinecraftConfig.activeMinecraftFolder = jsonClass.ActiveMinecraftFolderPath;
            MinecraftConfig.activeMinecraftAccount = jsonClass.ActiveMinecraftAccount;
            MinecraftConfig.isAutoJavaEnabled = jsonClass.IsAutoJavaEnabled;
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
