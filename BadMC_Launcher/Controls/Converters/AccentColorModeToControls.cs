using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.WinUI.Animations;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Controls.Converters;
public class AccentColorModeToControls : IValueConverter {

    // TODO: 已废弃, 改用Interactivity:Interaction.Behaviors.
    public IList<object> SystemItems { get; set; } = new List<object>();

    public IList<object> CustomItems { get; set; } = new List<object>();

    public IList<object> ImageMonetItems { get; set; } = new List<object>();

    public IList<object> ColorMonetItems { get; set; } = new List<object>();

    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is AccentColorModeEnum accentColor) {
            return accentColor switch {
                AccentColorModeEnum.System => SystemItems,
                AccentColorModeEnum.Custom => CustomItems,
                AccentColorModeEnum.ImageMonet => ImageMonetItems,
                AccentColorModeEnum.ColorMonet => ColorMonetItems,
                _ => new List<object>()
            };
        }
        else {
            return new List<object>();
        }
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is AccentColorModeEnum accentColor) {
            return accentColor switch {
                AccentColorModeEnum.System => SystemItems,
                AccentColorModeEnum.Custom => CustomItems,
                AccentColorModeEnum.ImageMonet => ImageMonetItems,
                AccentColorModeEnum.ColorMonet => ColorMonetItems,
                _ => new List<object>()
            };
        }
        else {
            return new List<object>();
        }
    }
}
