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
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Services.Configs;
public class SingleMinecraftConfigsService : ConfigClass {
    private readonly SingleMinecraftConfigs singleMinecraftConfigInstance = new();
    private readonly PathService pathService;

    public SingleMinecraftConfigsService(PathService _pathService) {
        pathService = _pathService;

        JvmArguments.ListChanged += OnListChanged;
    }

    public SingleMinecraftConfigsService() {
        pathService = App.GetService<PathService>();

        JvmArguments.ListChanged += OnListChanged;
    }

    public string? TargetMinecraftEntryPath {
        get => singleMinecraftConfigInstance.targetMinecraftEntryPath;
        set {
            if (singleMinecraftConfigInstance.targetMinecraftEntryPath != value) {
                singleMinecraftConfigInstance.targetMinecraftEntryPath = value;

                // Trigger Event
                OnPropertyChanged(nameof(TargetMinecraftEntryPath));

                // Write to Json or other logic
                SyncSettingSet();
            }
        }
    }

    public bool? IsAutoJavaEnabled {
        get => singleMinecraftConfigInstance.isAutoJavaEnabled;
        set {
            if (singleMinecraftConfigInstance.isAutoJavaEnabled != value) {
                singleMinecraftConfigInstance.isAutoJavaEnabled = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoJavaEnabled));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool? IsFullscreen {
        get => singleMinecraftConfigInstance.isFullscreen;
        set {
            if (singleMinecraftConfigInstance.isFullscreen != value) {
                singleMinecraftConfigInstance.isFullscreen = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsFullscreen));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool? IsEnableVersionIsolation {
        get => singleMinecraftConfigInstance.isEnableVersionIsolation;
        set {
            if (singleMinecraftConfigInstance.isEnableVersionIsolation != value) {
                singleMinecraftConfigInstance.isEnableVersionIsolation = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsEnableVersionIsolation));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool? IsAutoMemorySize {
        get => singleMinecraftConfigInstance.isAutoMemorySize;
        set {
            if (singleMinecraftConfigInstance.isAutoMemorySize != value) {
                singleMinecraftConfigInstance.isAutoMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoMemorySize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint? MaxMemorySize {
        get => singleMinecraftConfigInstance.maxMemorySize;
        set {
            if (singleMinecraftConfigInstance.maxMemorySize != value) {
                singleMinecraftConfigInstance.maxMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(MaxMemorySize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint? MinMemorySize {
        get => singleMinecraftConfigInstance.minMemorySize;
        set {
            if (singleMinecraftConfigInstance.minMemorySize != value) {
                singleMinecraftConfigInstance.minMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(MinMemorySize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string? JavaPath {
        get => singleMinecraftConfigInstance.javaPath;
        set {
            if (singleMinecraftConfigInstance.javaPath != value) {
                singleMinecraftConfigInstance.javaPath = value;

                // Trigger Event
                OnPropertyChanged(nameof(JavaPath));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string? LauncherName {
        get => singleMinecraftConfigInstance.launcherName;
        set {
            if (singleMinecraftConfigInstance.launcherName != value) {
                singleMinecraftConfigInstance.launcherName = value;

                // Trigger Event
                OnPropertyChanged(nameof(LauncherName));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public BindingList<string> JvmArguments {
        get => singleMinecraftConfigInstance.jvmArguments;
        set {
            if (!singleMinecraftConfigInstance.jvmArguments.SequenceEqual(value)) {
                singleMinecraftConfigInstance.jvmArguments.RaiseListChangedEvents = false;

                singleMinecraftConfigInstance.jvmArguments.Clear();
                singleMinecraftConfigInstance.jvmArguments.AddRange(value);

                singleMinecraftConfigInstance.jvmArguments.RaiseListChangedEvents = true;

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    private void OnListChanged(object? sender, ListChangedEventArgs e) {

        if (!SyncSettingSet()) {
            //TODO: Dialog
        }
    }

    public override bool SyncSettingGet() {
        if(TargetMinecraftEntryPath != null
            && File.Exists(Path.Combine(TargetMinecraftEntryPath, Path.Combine(AppDataPath.VersionConfigsPath, "MinecraftConfigs.json")))
            && pathService.TryReadConfig(Path.Combine(TargetMinecraftEntryPath, Path.Combine(AppDataPath.VersionConfigsPath, "MinecraftConfigs.json")), SingleMinecraftConfigsServiceContext.Default.SingleMinecraftConfigsService, out var jsonClass) && jsonClass != null) {
            singleMinecraftConfigInstance.isFullscreen = jsonClass.IsFullscreen;
            singleMinecraftConfigInstance.isEnableVersionIsolation = jsonClass.IsEnableVersionIsolation;
            singleMinecraftConfigInstance.isAutoMemorySize = jsonClass.IsAutoMemorySize;
            singleMinecraftConfigInstance.minMemorySize = jsonClass.MinMemorySize;
            singleMinecraftConfigInstance.maxMemorySize = jsonClass.MaxMemorySize;
            singleMinecraftConfigInstance.javaPath = jsonClass.JavaPath;
            singleMinecraftConfigInstance.launcherName = jsonClass.LauncherName;
            singleMinecraftConfigInstance.jvmArguments = jsonClass.JvmArguments;
            return true;
        }
        //TODO: Toast
        return false;
    }

    public override bool SyncSettingSet() {
        if (TargetMinecraftEntryPath != null) {
            return pathService.WriteConfig(Path.Combine(TargetMinecraftEntryPath, Path.Combine(AppDataPath.VersionConfigsPath, "MinecraftConfigs.json")), SingleMinecraftConfigsServiceContext.Default.SingleMinecraftConfigsService, this);
        }
        //TODO: Toast
        return false;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SingleMinecraftConfigsService))]
internal partial class SingleMinecraftConfigsServiceContext : JsonSerializerContext;
