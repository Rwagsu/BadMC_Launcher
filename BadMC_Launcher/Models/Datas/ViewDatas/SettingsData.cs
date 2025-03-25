using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls;


namespace BadMC_Launcher.Models.Datas.ViewDatas;

internal static class SettingsData {
    internal static DistinctiveItemBindingList<SettingsSideBarItem> SettingsSideBarItems { get; set; } = new();

    internal static DistinctiveItemBindingList<SettingsSideBarItem> SettingsSideBarFooterItems { get; set; } = new();
}
