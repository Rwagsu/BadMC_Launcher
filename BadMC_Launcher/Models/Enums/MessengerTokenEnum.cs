using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Models.Enums;
public enum MainPageMessengerTokenEnum {
    //Func Token
    PageNavigateToken,
    FlyoutPageNavigateToken,

    //GetValue Token
    MainSideBarFlyoutFrameToken,
    MainSideBarFrameToken,
    XamlRootToken
}

public enum MinecraftFolderContentDialogMessengerTokenEnum {
    //Func Token
    HideRenameFlyoutToken,
    ShowRenameFlyoutToken
}
