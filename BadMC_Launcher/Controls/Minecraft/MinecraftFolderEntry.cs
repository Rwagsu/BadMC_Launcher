using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Settings;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using Uno.Extensions.Specialized;

namespace BadMC_Launcher.Controls.Minecraft;

public partial class MinecraftFolderEntry : ObservableObject {
    private MinecraftParser? minecraftParser;
    private IEnumerable<MinecraftEntry> minecraftList = new List<MinecraftEntry>();

    public MinecraftFolderEntry() {
        MinecraftFolderId = "NewFolder";
        StarredMinecraftIds = new();
    }

    [ObservableProperty]
    public partial string MinecraftFolderId { get; set; }

    public required string MinecraftFolderPath { get; init; }

    [ObservableProperty]
    public partial string? ActiveMinecraftEntryId { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<string> StarredMinecraftIds { get; set; }

    public void StarMinecraftEntry() {

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
}
