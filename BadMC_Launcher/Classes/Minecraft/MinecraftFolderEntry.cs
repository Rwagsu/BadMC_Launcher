using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Settings;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using Uno.Extensions.Specialized;

namespace BadMC_Launcher.Classes.Minecraft;
public class MinecraftFolderEntry {
    private MinecraftParser? minecraftParser;
    private string minecraftFolderId = string.Empty;
    private string? activeMinecraftEntryId;
    private ObservableDataList<string> starredMinecraftIds = new();
    private IEnumerable<MinecraftEntry> minecraftList = new List<MinecraftEntry>();

    public MinecraftFolderEntry() {
    }
    public required string MinecraftFolderId {
        get => minecraftFolderId;
        set {
            minecraftFolderId = value;

            // Trigger Event
            OnPropertyChanged(nameof(MinecraftFolderId));
        }
    }

    public required string MinecraftFolderPath { get; init; }

    public string? ActiveMinecraftEntryId {
        get => activeMinecraftEntryId;
        set {
            activeMinecraftEntryId = value;

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftEntryId));
        }
    }

    public ObservableDataList<string> StarredMinecraftIds {
        get => starredMinecraftIds;
        set {
            starredMinecraftIds = value;

            // Trigger Event
            OnPropertyChanged(nameof(StarredMinecraftIds));
        }
    }

    public void SetMinecrafts() {
        // Get MinecraftParser
        minecraftParser = GetMinecraftParser();

        // Init MinecraftEntryList
        minecraftList = new List<MinecraftEntry>();

        // Get MinecraftEntries
        minecraftParser.GetMinecrafts().ForEach(item => ((List<MinecraftEntry>)minecraftList).Add(item));
    }

    public IEnumerable<MinecraftEntry> GetMinecrafts() {
        // If MinecraftEntryList is empty, get MinecraftEntries
        if (!minecraftList.Any()) {
            // Get MinecraftParser
            minecraftParser = GetMinecraftParser();

            // Init MinecraftEntryList
            minecraftList = new List<MinecraftEntry>();

            // Get MinecraftEntries
            minecraftParser.GetMinecrafts().ForEach(item => ( (List<MinecraftEntry>)minecraftList ).Add(item));
        }
        return minecraftList;
    }

    public MinecraftParser GetMinecraftParser() {
        if (minecraftParser == null) {
            minecraftParser = new MinecraftParser(MinecraftFolderPath);
        }
        return minecraftParser;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) {
        // Sync Setting
        App.GetService<MinecraftConfigService>().SyncSettingSet();

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
