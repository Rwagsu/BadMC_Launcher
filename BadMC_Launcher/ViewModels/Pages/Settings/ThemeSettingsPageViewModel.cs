using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;

namespace BadMC_Launcher.ViewModels.Pages.Settings;
public partial class ThemeSettingsPageViewModel : ObservableObject {
    private readonly ThemeConfigsService themeService = App.GetService<ThemeConfigsService>();
    
    public ThemeSettingsPageViewModel() {
        ThemeType = themeService.ThemeType;
        AccentColorHex = themeService.AccentColorHex;
        ViewAccentColorHex = "Resource \"AccentFillColorDefaultBrush\" Error";
        ImageMonetColors = new ObservableCollection<Brush>();
        ColorMonetColorHex = string.Empty;

        GetAccentColorHex();
    }

    // Theme
    [ObservableProperty]
    public partial AppTheme ThemeType { get; set; }

    // Accent Color
    [ObservableProperty]
    public partial string ViewAccentColorHex { get; set; }

    [ObservableProperty]
    public partial AccentColorModeEnum AccentColorMode { get; set; }

    [ObservableProperty]
    public partial string AccentColorHex { get; set; }

    // Image Monet Colors
    // TODO: 类型可能搞错了(
    [ObservableProperty]
    public partial ObservableCollection<Brush> ImageMonetColors { get; set; }

    // TODO: 类型可能又搞错了(
    [ObservableProperty]
    public partial Brush? SelectedImageMonetColor { get; set; }

    // Color Monet Colors
    // TODO: 类型还是可能搞错了(
    [ObservableProperty]
    public partial string ColorMonetColorHex { get; set; }

    [RelayCommand]
    private void SetThemeType() {
        themeService.ThemeType = ThemeType;
    }

    [RelayCommand]
    private void SetAccentColorHex() {
        themeService.AccentColorHex = AccentColorHex;
    }

    [RelayCommand]
    private void SetMonetAndApply(Brush parameter) {
        // TODO: 万恶的Monet
    }

    partial void OnAccentColorModeChanged(AccentColorModeEnum value) {
        themeService.AccentMode = value;
    }

    private void GetAccentColorHex() {
        var resource = Application.Current.Resources["AccentFillColorDefaultBrush"];
        if (resource is SolidColorBrush brush) {
            ViewAccentColorHex = brush.Color.ToHex();
        }
    }
}
