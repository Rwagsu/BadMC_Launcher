using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Services.Configs;
public class ThemeConfigsService : ConfigClass {
    private readonly PathService pathService;
    internal bool isSyncEnabled = false;

    public ThemeConfigsService(PathService _pathService) {
        pathService = _pathService;
    }

    public ThemeConfigsService() {
        pathService = App.GetService<PathService>();
    }

    public BackgroundTypeEnum BackgroundType {
        get => ThemeConfigs.backgroundType;
        set {
            if (ThemeConfigs.backgroundType != value) {
                ThemeConfigs.backgroundType = value;

                // Trigger Event
                OnPropertyChanged(nameof(BackgroundType));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public ThemeTypeEnum ThemeType { 
        get => ThemeConfigs.themeType;
        set {
            if (ThemeConfigs.themeType != value) {
                ThemeConfigs.themeType = value;

                // Trigger Event
                OnPropertyChanged(nameof(ThemeType));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string ImageBackgroundName {
        get => ThemeConfigs.imageBackgroundName;
        set {
            if (ThemeConfigs.imageBackgroundName != value) {
                ThemeConfigs.imageBackgroundName = value;

                // Trigger Event
                OnPropertyChanged(nameof(ImageBackgroundName));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public Stretch BackgroundStretch {
        get => ThemeConfigs.backgroundStretch;
        set {
            if (ThemeConfigs.backgroundStretch != value) {
                ThemeConfigs.backgroundStretch = value;

                // Trigger Event
                OnPropertyChanged(nameof(BackgroundStretch));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string SolidColorBackgroundCode {
        get => ThemeConfigs.solidColorBackgroundCode;
        set {
            if (ThemeConfigs.solidColorBackgroundCode != value) {
                ThemeConfigs.solidColorBackgroundCode = value;

                // Trigger Event
                OnPropertyChanged(nameof(SolidColorBackgroundCode));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string WindowName {
        get => ThemeConfigs.windowName;
        set {
            if (ThemeConfigs.windowName != value) {
                ThemeConfigs.windowName = value;

                // Trigger Event
                OnPropertyChanged(nameof(WindowName));
            }
        }
    }

    public async Task<Brush> SetBackground(BackgroundTypeEnum backgroundType) {
        if (App.GetService<PathService>().CheckPath(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers"))) {
            if (!Directory.Exists(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers"))) {
                Directory.CreateDirectory(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers"));
            }
            switch (backgroundType) {
                case BackgroundTypeEnum.StaticImage:
                    if (string.IsNullOrWhiteSpace(ThemeConfigs.imageBackgroundName)) {
                        //TODO :Dialog EXCEPTION
                        return SetBrush(Windows.UI.Color.FromArgb(0, 119, 255, 1)); ;
                    }
                    return SetBrush(new BitmapImage(new Uri(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers", ThemeConfigs.imageBackgroundName))));
                case BackgroundTypeEnum.BingWallpaper:
                    return SetBrush(new BitmapImage(new Uri(await GetBingWallpaperUrl())));
                case BackgroundTypeEnum.Acrylic:
                    //TODO: Only MacOS
                    return SetBrush(Windows.UI.Color.FromArgb(0, 119, 255, 1));
                default:
                    var color = ColorTranslator.FromHtml(ThemeConfigs.solidColorBackgroundCode);
                    return SetBrush(Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B));
            }
        }
        return SetBrush(Windows.UI.Color.FromArgb(0, 119, 255, 1));
    }

    public override bool SyncSettingGet() {
        if (pathService.TryReadConfig(Path.Combine(AppDataPath.pathsList["ConfigsPath"], @"ThemeConfigs.json"), ThemeConfigsServiceContext.Default.ThemeConfigsService, out var jsonClass) && jsonClass != null) {
            ThemeConfigs.backgroundType = jsonClass.BackgroundType;
            ThemeConfigs.themeType = jsonClass.ThemeType;
            ThemeConfigs.imageBackgroundName = jsonClass.ImageBackgroundName;
            ThemeConfigs.backgroundStretch = jsonClass.BackgroundStretch;
            ThemeConfigs.solidColorBackgroundCode = jsonClass.SolidColorBackgroundCode;
            ThemeConfigs.windowName = jsonClass.WindowName;
            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (pathService.WriteConfig(Path.Combine(AppDataPath.pathsList["ConfigsPath"], @"ThemeConfigs.json"), ThemeConfigsServiceContext.Default.ThemeConfigsService, this)) {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<ThemeConfigsService>(this), "MinecraftConfigChanged");
            return true;
        }
        return false;
    }
    public Brush SetBrush<T>(T color) {
        if (typeof(T) == typeof(Windows.UI.Color) && color != null) {
            return new SolidColorBrush((Windows.UI.Color)(object)color);
        }
        if (typeof(T) == typeof(BitmapImage) && color != null) {
            return new ImageBrush() {
                ImageSource = (BitmapImage)(object)color,
                Stretch = ThemeConfigs.backgroundStretch
            };
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
[JsonSerializable(typeof(ThemeConfigsService))]
internal partial class ThemeConfigsServiceContext : JsonSerializerContext;
