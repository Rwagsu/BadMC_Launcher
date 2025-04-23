using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Models.Datas;
using BadMC_Launcher.Models.Datas.SettingsDatas;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Services.Settings;
public class ThemeSettingService : ConfigClass {
    internal bool isSyncEnabled = false;

    public BackgroundTypeEnum BackgroundType {
        get => ThemeSetting.backgroundType;
        set {
            ThemeSetting.backgroundType = value;

            // Trigger Event
            OnPropertyChanged(nameof(BackgroundType));

            // Sync Setting
            SyncSettingSet();

            // Set Background
            SetBackground();
        }
    }

    public ThemeTypeEnum ThemeType { 
        get => ThemeSetting.themeType;
        set {
            ThemeSetting.themeType = value;

            // Trigger Event
            OnPropertyChanged(nameof(ThemeType));

            // Sync Setting
            SyncSettingSet();
        }
    }

    public string ImageBackgroundName {
        get => ThemeSetting.imageBackgroundName;
        set {
            ThemeSetting.imageBackgroundName = value;

            // Trigger Event
            OnPropertyChanged(nameof(ImageBackgroundName));

            // Sync Setting
            SyncSettingSet();

            // Set Background
            SetBackground();
        }
    }

    public Stretch BackgroundStretch {
        get => ThemeSetting.backgroundStretch;
        set {
            ThemeSetting.backgroundStretch = value;

            // Trigger Event
            OnPropertyChanged(nameof(BackgroundStretch));

            // Sync Setting
            SyncSettingSet();

            // Set Background
            SetBackground();
        }
    }

    public string SolidColorBackgroundCode {
        get => ThemeSetting.solidColorBackgroundCode;
        set {
            ThemeSetting.solidColorBackgroundCode = value;

            // Trigger Event
            OnPropertyChanged(nameof(SolidColorBackgroundCode));

            // Sync Setting
            SyncSettingSet();

            // Set Background
            SetBackground();
        }
    }

    public string WindowName {
        get => ThemeSetting.windowName;
        set {
            ThemeSetting.windowName = value;

            // Trigger Event
            OnPropertyChanged(nameof(WindowName));

            // Sync Setting
            SyncSettingSet();
        }
    }

    public async void SetBackground(Action<Brush>? backgroundChanged = null) {
        if (backgroundChanged == null) {
            //TODO: 这应该得从代码介入了，应该得Dialog(
            return;
        }
        App.GetService<FileService>().CheckFolderAndFile(Path.Combine(AppDataPath.AssetsPath, "Wallpapers"), false);
        switch (ThemeSetting.backgroundType) {
            case BackgroundTypeEnum.SolidColor:
                var color = ColorTranslator.FromHtml(ThemeSetting.solidColorBackgroundCode);
                    SetBrush(Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B), backgroundChanged);
                break;
            case BackgroundTypeEnum.StaticImage:
                if (string.IsNullOrWhiteSpace(ThemeSetting.imageBackgroundName)) {
                    //TODO :Dialog EXCEPTION
                    return;
                }
                SetBrush(new BitmapImage(new Uri(Path.Combine(AppDataPath.AssetsPath, "Wallpapers", ThemeSetting.imageBackgroundName))), backgroundChanged);
                break;
            case BackgroundTypeEnum.BingWallpaper:
                SetBrush(new BitmapImage(new Uri(await GetBingWallpaperUrl())), backgroundChanged);
                break;
            case BackgroundTypeEnum.Acrylic:
                //TODO: Only MacOS
                break;
        }
    }

    public override bool SyncSettingGet() {
        if (App.GetService<FileService>().ReadConfig(Path.Combine(AppDataPath.ConfigsPath, @"Settings\ThemeSettings.json"), ThemeSettingServiceContext.Default.ThemeSettingService, out var jsonClass) && jsonClass != null) {
            ThemeSetting.backgroundType = jsonClass.BackgroundType;
            ThemeSetting.themeType = jsonClass.ThemeType;
            ThemeSetting.imageBackgroundName = jsonClass.ImageBackgroundName;
            ThemeSetting.backgroundStretch = jsonClass.BackgroundStretch;
            ThemeSetting.solidColorBackgroundCode = jsonClass.SolidColorBackgroundCode;
            ThemeSetting.windowName = jsonClass.WindowName;
            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (App.GetService<FileService>().WriteConfig(Path.Combine(AppDataPath.ConfigsPath, @"Settings\ThemeSettings.json"), ThemeSettingServiceContext.Default.ThemeSettingService, this)) {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<ThemeSettingService>(this), "MinecraftConfigChanged");
            return true;
        }
        return false;
    }
    public void SetBrush<T>(T color, Action<Brush> backgroundChanged) {
        if (typeof(T) == typeof(Windows.UI.Color) && color != null) {
            backgroundChanged(new SolidColorBrush((Windows.UI.Color)(object)color));
            return;
        }
        if (typeof(T) == typeof(BitmapImage) && color != null) {
            backgroundChanged(new ImageBrush() {
                ImageSource = (BitmapImage)(object)color,
                Stretch = ThemeSetting.backgroundStretch
            });
            return;
        }
        throw new InvalidOperationException("Unsupported type for SetBrushAsync");
    }

    public static async Task<string> GetBingWallpaperUrl() {
        try {
            var jsonText = await App.GetService<HttpClient>().GetStringAsync("https://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-CN");
            using JsonDocument doc = JsonDocument.Parse(jsonText);
            var status = doc.RootElement.TryGetProperty("images", out var imagesjsonElement);
            status = imagesjsonElement[0].TryGetProperty("url", out var urljsonElement);
            if (status == true) {
                // TODO: Toast Exception
                return "https://cn.bing.com" + urljsonElement.GetString() ?? throw new Exception("Can't get Bing wallpapers.");
            }
        }
        catch (Exception ex) {
            switch (ex) {
                case HttpRequestException:
                case TaskCanceledException:
                    //TODO: Toast Exception
                    break;
                case JsonException:
                    //TODO: Toast Exception
                    break;
                default:
                    throw;
            }
        }
        //TODO: Toast Exception
        throw new Exception("Can't get Bing wallpapers.");
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ThemeSettingService))]
internal partial class ThemeSettingServiceContext : JsonSerializerContext;
