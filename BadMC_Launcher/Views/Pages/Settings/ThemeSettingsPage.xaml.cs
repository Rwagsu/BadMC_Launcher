using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Models.Enums;
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
public sealed partial class ThemeSettingsPage : Page {
    private readonly ThemeSettingsPageViewModel viewModel = new ThemeSettingsPageViewModel();
    public ThemeSettingsPage() {
        this.InitializeComponent();
        DataContext = viewModel;
    }

    private string GetAccentSettingsCardHeader(int resourceIndex) {
        var loader =  App.GetService<ResourceLoader>();
        return resourceIndex switch {
            0 => loader.GetString("ThemeSettingsPage_SettingsAccentColor_SettingsCardHeader"),
            1 => loader.GetString("ThemeSettingsPage_SolidColorAccentColor_SettingsCardHeader"),
            2 => loader.GetString("ThemeSettingsPage_ImageMonetAccentColor_SettingsCardHeader"),
            _ => string.Empty,
        };
    }
    private string GetAccentSettingsCardDescription(int resourceIndex) {
        var loader =  App.GetService<ResourceLoader>();
        return resourceIndex switch {
            0 => loader.GetString("ThemeSettingsPage_SettingsAccentColor_SettingsCardDescription"),
            1 => loader.GetString("ThemeSettingsPage_SolidColorAccentColor_SettingsCardDescription"),
            2 => loader.GetString("ThemeSettingsPage_ImageMonetAccentColor_SettingsCardDescription"),
            _ => string.Empty
        };
    }

    private FontIcon GetAccentSettingsCardIcon(int resourceIndex) {
        var loader =  App.GetService<ResourceLoader>();
        return resourceIndex switch {
            0 => new FontIcon() { Glyph = "\uEB68" },
            1 => new FontIcon() { Glyph = "\uE790" },
            2 => new FontIcon() { Glyph = "\uEE71" },
            _ => new FontIcon() { Glyph = "\uE790" }
        };
    }
}
