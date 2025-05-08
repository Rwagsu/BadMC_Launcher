using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Classes.Converters;
internal class DoubleToUIntConverter: IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string culture) {
        return value; // uint → double（正常情况）
    }

    public object ConvertBack(object value, Type targetType, object parameter, string culture) {
        if (value is double d && double.IsNaN(d))
            return 0u; // 如果输入 NaN，返回 0
        if (value is double d2)
            return (uint)d2; // 强制转换（会截断小数）
        return 0u;
    }
}
