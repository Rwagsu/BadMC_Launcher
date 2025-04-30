using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Hardware.Info;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using Uno.UI.RemoteControl.Host;
using Windows.Graphics;

namespace BadMC_Launcher.Models.Datas;

public static class AppParameters {
    public static SizeInt32 WindowSize { get; } = new() {
        Width = 1024,
        Height = 620
    };

    public static HardwareInfo SystemInfo { get; } = new();
}
