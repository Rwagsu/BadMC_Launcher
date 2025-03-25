using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Controls;

namespace BadMC_Launcher.Models.Datas.ViewDatas;

internal static class MainMenuData {
    internal static DistinctiveItemBindingList<IMainMenuSharchFilterItem> MainMenuSharchFilterItems { get; set; } = new();

    internal static DistinctiveItemBindingList<MainMenuItem> MainMenuItems { get; set; } = new();
}
