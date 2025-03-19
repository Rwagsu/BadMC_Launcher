using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Models.Datas.Mappings;
public static class UpdateMapping {
    public readonly static Dictionary<string, string> MinecraftConfig = new() {
        { "MinecraftPaths", "MinecraftFolders" }
    };
}
