using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Controls.Converters;

class StringPathToImageConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is string path && !string.IsNullOrEmpty(path)) {
            // Convert hex string back to Color
            return path.FindImageFromBackground();
        }
        // Default to white if the value is not a valid hex string
        return new BitmapImage();
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is BitmapImage image) {
            try {
                return Path.GetFileName(image.UriSource.LocalPath);
            }
            catch (ArgumentException) {
                
            }
        }
        // Default to white if the value is not a Color
        return string.Empty;
    }
}
