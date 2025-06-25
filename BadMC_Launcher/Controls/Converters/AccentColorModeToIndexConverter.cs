using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Controls.Converters;

public class AccentColorModeToIndexConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is AccentColorModeEnum accentColorMode) {
            // Convert ThemeTypeEnum to an index
            return accentColorMode switch {
                AccentColorModeEnum.System => 0,
                AccentColorModeEnum.Custom => 1,
                AccentColorModeEnum.ImageMonet => 2,
                AccentColorModeEnum.ColorMonet => 3,
                _ => 0
            };
        }

        // If the value is not a ThemeTypeEnum, return 0 as a default index
        return 0;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is int accentIndex) {
            // Convert index back to ThemeTypeEnum
            return accentIndex switch {
                0 => AccentColorModeEnum.System,
                1 => AccentColorModeEnum.Custom,
                2 => AccentColorModeEnum.ImageMonet,
                3 => AccentColorModeEnum.ColorMonet,
                _ => 0
            };
        }

        // If the value is not an int, return 0 as a default ThemeTypeEnum
        return AccentColorModeEnum.System;
    }
}
