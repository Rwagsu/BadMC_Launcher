using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Interfaces;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Classes.DataClasses.VersionIsolationFilters;

public readonly struct DisabledVersionFilter : IVersionIsolationFilter {
    public DisabledVersionFilter() {
        Name = "DisabledVersionFilter";
    }

    public string Name { get; }

    public bool IsVersionIsolation(MinecraftEntry _) => false;
}
