using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinecraftLaunch.Base.Models.Authentication;
using Newtonsoft.Json.Linq;
using MinecraftLaunch.Base.Models.Game;
using Microsoft.Windows.ApplicationModel.Resources;
using BadMC_Launcher.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using NbtToolkit;
using System.Xml.Linq;
using System.ComponentModel;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Models.Datas;
using BadMC_Launcher.Models.Datas.Mappings;

namespace BadMC_Launcher.Servicess.Settings;
public class MinecraftConfigService : IConfigClass {
    internal bool isSyncEnabled = false;

    public MinecraftConfigService() {
        if (MinecraftConfig.minecraftAccounts is ObservableDataList<Account> minecraftAccountsObservableDataList
            && MinecraftConfig.javaPaths is ObservableDataList<string> javaPathsObservableDataList
            && MinecraftConfig.minecraftPaths is ObservableDataList<MinecraftFolderEntry> minecraftPathsObservableDataList) {

            //Triggers an event when a property is changed
            minecraftAccountsObservableDataList.CollectionChanged += OnCollectionChanged;
            javaPathsObservableDataList.CollectionChanged += OnCollectionChanged;
            minecraftPathsObservableDataList.CollectionChanged += OnCollectionChanged;
        }
    }

    public IEnumerable<Account> MinecraftAccounts {
        get => MinecraftConfig.minecraftAccounts;
        set {
            MinecraftConfig.minecraftAccounts = value;

            //Write to Json
            SyncSettingSet();
        }
    }
    public IEnumerable<string> JavaPaths {
        get => MinecraftConfig.javaPaths;
        set {
            MinecraftConfig.javaPaths = value;

            //Write to Json
            SyncSettingSet();
        }
    }
    public IEnumerable<MinecraftFolderEntry> MinecraftFolders {
        get => MinecraftConfig.minecraftPaths;
        set {
            MinecraftConfig.minecraftPaths = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public JavaEntry? ActiveJavaPath {
        get => MinecraftConfig.activeJavaPath;
        set {
            MinecraftConfig.activeJavaPath = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? ActiveMinecraftFolderPath {
        get => MinecraftConfig.activeMinecraftFolder;
        set {
            MinecraftConfig.activeMinecraftFolder = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public Account? ActiveMinecraftAccount {
        get => MinecraftConfig.activeMinecraftAccount;
        set {
            MinecraftConfig.activeMinecraftAccount = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsFullscreen {
        get => MinecraftConfig.isFullscreen;
        set {
            MinecraftConfig.isFullscreen = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsEnableIndependencyCore {
        get => MinecraftConfig.isEnableIndependencyCore;
        set {
            MinecraftConfig.isEnableIndependencyCore = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool IsAutoMemorySize {
        get => MinecraftConfig.isAutoMemorySize;
        set {
            MinecraftConfig.isAutoMemorySize = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public int MinMemorySize {
        get => MinecraftConfig.minMemorySize;
        set {
            MinecraftConfig.minMemorySize = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public int MaxMemorySize {
        get => MinecraftConfig.maxMemorySize;
        set {
            MinecraftConfig.maxMemorySize = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? LauncherName {
        get => MinecraftConfig.launcherName;
        set {
            MinecraftConfig.launcherName = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    public IEnumerable<string>? JvmArguments {
        get => MinecraftConfig.jvmArguments;
        set {
            MinecraftConfig.jvmArguments = value;

            //Write to Json
            SyncSettingSet();
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {

        if (!SyncSettingSet()) {
            //TODO: Dialog
        }
    }

    public bool SyncSettingGet() {
        if (App.GetService<FileService>().ReadConfig(Path.Combine(AppDataPath.ConfigsPath, "MinecraftConfigs.json"), MinecraftConfigServiceContext.Default.MinecraftConfigService, out var jsonClass, UpdateMapping.MinecraftConfig) && jsonClass != null) {
            //TODO: 解蜜
            MinecraftConfig.minecraftAccounts = jsonClass.MinecraftAccounts;
            MinecraftConfig.javaPaths = jsonClass.JavaPaths;
            MinecraftConfig.minecraftPaths = jsonClass.MinecraftFolders;
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

    private void PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (!SyncSettingSet()) {
            //TODO: Dialog
        }
    }

    public bool SyncSettingSet() {
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
