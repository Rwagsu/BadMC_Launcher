using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Controls.Converters;

class ThemeTypeToIndexConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is AppTheme themeType) {
            // Convert ThemeTypeEnum to an index
            return themeType switch {
                AppTheme.System => 0,
                AppTheme.Light => 1,
                AppTheme.Dark => 2,
                _ => 0
            };
        }
        // If the value is not a ThemeTypeEnum, return 0 as a default index
        return 0;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is int themeIndex) {
            // Convert index back to ThemeTypeEnum
            return themeIndex switch {
                0 => AppTheme.System,
                1 => AppTheme.Light,
                2 => AppTheme.Dark,
                _ => 0
            };
        }
        // If the value is not an int, return 0 as a default ThemeTypeEnum
        return AppTheme.System;
    }
}
