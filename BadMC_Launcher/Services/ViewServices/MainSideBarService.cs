using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Datas.ViewDatas;
using BadMC_Launcher.ViewModels.Pages;
using BadMC_Launcher.Classes;
using Microsoft.Windows.ApplicationModel.Resources;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Interfaces;

namespace BadMC_Launcher.Services.ViewServices;
public class MainSideBarService {
    public void NavigateToPage<T>() where T : Page {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Type>(typeof(T)), MainPageMessengerTokenEnum.PageNavigateToken.ToString());
    }

    public void Register(MainSideBarItem mainSideBarItem, bool isFooter = false) {
        if (isFooter) {
            MainSideBarData.MainSideBarFooterItems.Add(mainSideBarItem);
        }
        else {
            MainSideBarData.MainSideBarItems.Add(mainSideBarItem);
        }
    }

    public void SearchFilterRegister(IMainMenuSharchFilterItem mainMenuSharchItem) {
        MainSideBarData.MainMenuSharchFilterItems.Add(mainMenuSharchItem);
    }

    public void MenuItemRegister(MainMenuItem mainMenuItem) {
        MainSideBarData.MainMenuItems.Add(mainMenuItem);
    }
}
