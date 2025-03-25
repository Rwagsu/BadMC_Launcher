using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.Controls;

namespace BadMC_Launcher.Services.ViewServices;

public class MainMenuService {
    public void SearchFilterRegister(IMainMenuSharchFilterItem mainMenuSharchItem) {
        MainMenuData.MainMenuSharchFilterItems.Add(mainMenuSharchItem);
    }

    public void MenuItemRegister(MainMenuItem mainMenuItem) {
        MainMenuData.MainMenuItems.Add(mainMenuItem);
    }
}
