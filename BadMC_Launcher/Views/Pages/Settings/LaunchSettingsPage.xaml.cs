using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Models.Datas;
using BadMC_Launcher.ViewModels.Pages.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BadMC_Launcher.Views.Pages.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LaunchSettingsPage : Page {
    private LaunchSettingsPageViewModel viewModel => (LaunchSettingsPageViewModel)DataContext;
    public LaunchSettingsPage() {
		InitializeComponent();
        DataContext = new LaunchSettingsPageViewModel();
    }

    private string GetStringGameMemory(object gameMemory, object usedMemory) {
        if (gameMemory is double gameNumber && usedMemory is double usedNumber) {
            return Math.Round(gameNumber - usedNumber, 2).ToString();
        }
        return string.Empty;
    }
    private double GetProgressRingMemoryView(object memory, object maxMemory) {
        if (memory is double number && maxMemory is double maxNumber) {
            return ( number / maxNumber ) * 100.0;
        }
        return 0.0;
    }

    private string GetGameMemoryScore(object gameMemory, object usedMemory) {
        if (gameMemory is double numberGameMemory && usedMemory is double numberUsedMemory) {
            var loader = App.GetService<ResourceLoader>();
            var memory = numberGameMemory - numberUsedMemory;
            if (memory <= 0.0) {
                return loader.GetString("LaunchSettingsPage_MemoryScore0");
            }
            else if (memory < 1.0) {
                return loader.GetString("LaunchSettingsPage_MemoryScore1");

            }
            else if (memory < 4.0) {
                return loader.GetString("LaunchSettingsPage_MemoryScore2");

            }
            else if (memory < 16.0) {
                return loader.GetString("LaunchSettingsPage_MemoryScore3");

            }
            else if (memory < 32.0) {
                return loader.GetString("LaunchSettingsPage_MemoryScore4");

            }
            return loader.GetString("LaunchSettingsPage_MemoryScore5");
        }
        return string.Empty;
    }
}

