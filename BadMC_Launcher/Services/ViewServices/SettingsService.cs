using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.Controls;

namespace BadMC_Launcher.Services.ViewServices;

public class SettingsService {
    public void SideBarRegister(SettingsSideBarItem settingsSideBarItem, bool isFooter = false) {
        if (isFooter) {
            SettingsData.SettingsSideBarFooterItems.Add(settingsSideBarItem);
        }
        else {
            SettingsData.SettingsSideBarItems.Add(settingsSideBarItem);
        }
    }
}
