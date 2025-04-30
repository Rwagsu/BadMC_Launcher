

using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Controls.Converters;
public class SSuffixAppendConverter : IValueConverter {

    public object Convert(object value, Type targetType, object parameter, string culture) {
        if (value is double number && parameter is double parameterNumber) {
            return ( number / parameterNumber ) * 100.0;
        }
        return value;
    }


    public object ConvertBack(object value, Type targetType, object parameter, string culture) {
        if (value is double number && parameter is double parameterNumber) {
            return ( number / 100.0 ) * parameterNumber;
        }
        return value;
    }
}
