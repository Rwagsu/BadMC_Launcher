using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.Minecraft;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Extensions;
public static class JavaEntryExtension {
    public static BitmapImage GetJavaIcon(this JavaEntry javaEntry) {
        var ImagePath = $"ms-appx:///Assets/Icons/JavaIcons/{javaEntry.JavaType.ToLower()}.png";

        if (File.Exists(ImagePath)) {
            return new BitmapImage(new Uri(ImagePath));
        }
        return new BitmapImage(new Uri("ms-appx:///Assets/Icons/JavaIcons/openjdk.png"));
    }
}
