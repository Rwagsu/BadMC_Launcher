using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.Minecraft;
using BadMC_Launcher.Servicess.Settings;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;
public partial class MinecraftFolderContentDialogViewModel : ObservableObject {
    private MinecraftConfigService minecraftCOnfigService = App.GetService<MinecraftConfigService>();

    public MinecraftFolderContentDialogViewModel() {

        MinecraftFoldersList = minecraftCOnfigService.MinecraftFolders.ToObservableCollection();
    }

    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderEntry> MinecraftFoldersList { get; set; }
}
