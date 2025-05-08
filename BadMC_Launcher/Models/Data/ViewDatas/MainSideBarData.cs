using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Interfaces;

namespace BadMC_Launcher.Models.Data.ViewData;
internal static class MainSideBarData {
    internal static DistinctiveItemBindingList<MainSideBarItem> MainSideBarItems { get; set; } = new();

    internal static DistinctiveItemBindingList<MainSideBarItem> MainSideBarFooterItems { get; set; } = new();

    internal static DistinctiveItemBindingList<IMainMenuSharchFilterItem> MainMenuSharchFilterItems { get; set; } = new();

    internal static DistinctiveItemBindingList<MainMenuItem> MainMenuItems { get; set; } = new();
}
