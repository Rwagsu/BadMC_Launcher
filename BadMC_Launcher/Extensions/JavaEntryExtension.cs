using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.Minecraft;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;
using Uno.Disposables;

namespace BadMC_Launcher.Extensions;
public static class JavaEntryExtension {
    public async static Task<string> GetJavaIconPathAsync(this JavaEntry javaEntry) {
        var ImagePath = $"ms-appx:///Assets/Icons/JavaIcons/{javaEntry.JavaType.ToLower()}.png";

        if (await StorageFile.GetFileFromApplicationUriAsync(new Uri(ImagePath)) != null) {
            return ImagePath;
        }
        return "ms-appx:///Assets/Icons/JavaIcons/openjdk.png";
    }
}
