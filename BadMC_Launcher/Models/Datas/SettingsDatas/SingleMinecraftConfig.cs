using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Models.Datas.SettingsDatas;
internal class SingleMinecraftConfig {
    internal string? targetMinecraftEntryPath;

    internal bool? isFullscreen;

    internal bool? isEnableIndependencyCore;

    internal bool? isAutoMemorySize;

    internal int? minMemorySize;

    internal int? maxMemorySize;

    internal JavaEntry? javaPath;

    internal string? launcherName;

    internal ObservableDataList<string> jvmArguments = new();
}
