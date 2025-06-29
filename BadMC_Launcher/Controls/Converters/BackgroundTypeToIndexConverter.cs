using BadMC_Launcher.Models.Enums;
using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Controls.Converters;

public class BackgroundTypeToIndexConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is BackgroundTypeEnum backgroundType) {
            // Convert BackgroundTypeEnum to an index
            return backgroundType switch {
                BackgroundTypeEnum.StaticImage => 0,
                BackgroundTypeEnum.BingWallpaper => 1,
                BackgroundTypeEnum.Acrylic => 2,
                BackgroundTypeEnum.SolidColor => 3,
                _ => 0
            };
        }

        // If the value is not a BackgroundTypeEnum, return 0 as a default index
        return 0;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is int accentIndex) {
            // Convert index back to BackgroundTypeEnum
            return accentIndex switch {
                0 => BackgroundTypeEnum.StaticImage,
                1 => BackgroundTypeEnum.BingWallpaper,
                2 => BackgroundTypeEnum.Acrylic,
                3 => BackgroundTypeEnum.SolidColor,
                _ => 0
            };
        }

        // If the value is not an int, return 0 as a default BackgroundTypeEnum
        return BackgroundTypeEnum.SolidColor;
    }
}
