using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Interfaces;

namespace BadMC_Launcher.Services.Settings;

class LaunchSettingsService {
    public HashSet<IVersionIsolationFilter> VersionIsolationFilter { get; set; } = new HashSet<IVersionIsolationFilter>();
}
