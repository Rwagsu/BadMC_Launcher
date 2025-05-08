using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Models.Data.SettingsData;
internal class SingleMinecraftConfig {
    internal string? targetMinecraftEntryPath;

    internal bool? isAutoJavaEnabled;

    internal bool? isFullscreen;

    internal bool? isEnableIndependencyCore;

    internal bool? isAutoMemorySize;
    
    internal uint? maxMemorySize;

    internal uint? minMemorySize;

    internal string? javaPath;

    internal string? launcherName;

    internal BindingList<string> jvmArguments = new();
}
