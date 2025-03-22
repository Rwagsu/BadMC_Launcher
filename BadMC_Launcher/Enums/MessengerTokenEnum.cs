using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Enums;
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
    //GetValue Token
    RenameFlyoutToken,
}


public enum MinecraftConfigMessengerTokenEnum {
    ActiveMinecraftFolder
}
