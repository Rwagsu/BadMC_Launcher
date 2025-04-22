using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Services.Settings;
public class SingleMinecraftConfigService : ConfigClass {
    private SingleMinecraftConfig singleMinecraftConfigInstance = new();

    public SingleMinecraftConfigService() {
        JvmArguments.ListChanged += OnListChanged;
    }

    public string? TargetMinecraftEntryPath {
        get => singleMinecraftConfigInstance.targetMinecraftEntryPath;
        set {
            singleMinecraftConfigInstance.targetMinecraftEntryPath = value;

            // Trigger Event
            OnPropertyChanged(nameof(TargetMinecraftEntryPath));

            //Write to Json or other logic
            SyncSettingSet();
        }
    }

    public bool IsAutoJavaEnabled {
        get => singleMinecraftConfigInstance.isAutoJavaEnabled;
        set {
            singleMinecraftConfigInstance.isAutoJavaEnabled = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsAutoJavaEnabled));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool? IsFullscreen {
        get => singleMinecraftConfigInstance.isFullscreen;
        set {
            singleMinecraftConfigInstance.isFullscreen = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsFullscreen));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool? IsEnableIndependencyCore {
        get => singleMinecraftConfigInstance.isEnableIndependencyCore;
        set {
            singleMinecraftConfigInstance.isEnableIndependencyCore = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsEnableIndependencyCore));

            //Write to Json
            SyncSettingSet();
        }
    }

    public bool? IsAutoMemorySize {
        get => singleMinecraftConfigInstance.isAutoMemorySize;
        set {
            singleMinecraftConfigInstance.isAutoMemorySize = value;

            // Trigger Event
            OnPropertyChanged(nameof(IsAutoMemorySize));

            //Write to Json
            SyncSettingSet();
        }
    }

    public int? MinMemorySize {
        get => singleMinecraftConfigInstance.minMemorySize;
        set {
            singleMinecraftConfigInstance.minMemorySize = value;

            // Trigger Event
            OnPropertyChanged(nameof(MinMemorySize));

            //Write to Json
            SyncSettingSet();
        }
    }

    public int? MaxMemorySize {
        get => singleMinecraftConfigInstance.maxMemorySize;
        set {
            singleMinecraftConfigInstance.maxMemorySize = value;

            // Trigger Event
            OnPropertyChanged(nameof(MaxMemorySize));

            //Write to Json
            SyncSettingSet();
        }
    }

    public JavaEntry? JavaPath {
        get => singleMinecraftConfigInstance.javaPath;
        set {
            singleMinecraftConfigInstance.javaPath = value;

            // Trigger Event
            OnPropertyChanged(nameof(JavaPath));

            //Write to Json 
            SyncSettingSet();
        }
    }

    public string? LauncherName {
        get => singleMinecraftConfigInstance.launcherName;
        set {
            singleMinecraftConfigInstance.launcherName = value;

            // Trigger Event
            OnPropertyChanged(nameof(LauncherName));

            //Write to Json
            SyncSettingSet();
        }
    }

    public BindingList<string> JvmArguments {
        get => singleMinecraftConfigInstance.jvmArguments;
        set {
            singleMinecraftConfigInstance.jvmArguments.RaiseListChangedEvents = false;

            singleMinecraftConfigInstance.jvmArguments.Clear();
            singleMinecraftConfigInstance.jvmArguments.AddRange(value);

            singleMinecraftConfigInstance.jvmArguments.RaiseListChangedEvents = true;

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
        if (TargetMinecraftEntryPath != null) {
            if (File.Exists(Path.Combine(TargetMinecraftEntryPath, @"BadBCConfigs\MinecraftConfig.json"))) {
                if(App.GetService<FileService>().ReadConfig(Path.Combine(TargetMinecraftEntryPath, @"BadBCConfigs\MinecraftConfig.json"), SingleMinecraftConfigServiceContext.Default.SingleMinecraftConfigService, out var jsonClass) && jsonClass != null) {
                    singleMinecraftConfigInstance.isFullscreen = jsonClass.IsFullscreen;
                    singleMinecraftConfigInstance.isEnableIndependencyCore = jsonClass.IsEnableIndependencyCore;
                    singleMinecraftConfigInstance.isAutoMemorySize = jsonClass.IsAutoMemorySize;
                    singleMinecraftConfigInstance.minMemorySize = jsonClass.MinMemorySize;
                    singleMinecraftConfigInstance.maxMemorySize = jsonClass.MaxMemorySize;
                    singleMinecraftConfigInstance.javaPath = jsonClass.JavaPath;
                    singleMinecraftConfigInstance.launcherName = jsonClass.LauncherName;
                    singleMinecraftConfigInstance.jvmArguments = jsonClass.JvmArguments;
                    return true;
                }
            }
        }
        //TODO: Toast
        return false;
    }

    public override bool SyncSettingSet() {
        if (TargetMinecraftEntryPath != null) {
            return App.GetService<FileService>().WriteConfig(Path.Combine(TargetMinecraftEntryPath, @"BadBCConfigs\MinecraftConfig.json"), SingleMinecraftConfigServiceContext.Default.SingleMinecraftConfigService, this);
        }
        //TODO: Toast
        return false;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SingleMinecraftConfigService))]
internal partial class SingleMinecraftConfigServiceContext : JsonSerializerContext;
