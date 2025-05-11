using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Interfaces;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Classes.DataClasses.VersionIsolationFilters;

public readonly struct ModLoaderVersionFilter : IVersionIsolationFilter {
    public ModLoaderVersionFilter() {
        Name = "ModLoaderVersionFilter";
    }

    public string Name { get; }

    public bool IsVersionIsolation(MinecraftEntry entry) {
        return entry is ModifiedMinecraftEntry modifiedEntry && modifiedEntry.ModLoaders.Any(item => item.Type != MinecraftLaunch.Base.Enums.ModLoaderType.OptiFine || item.Type != MinecraftLaunch.Base.Enums.ModLoaderType.Unknown);
    }
}
