using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Extensions;
public static class ConverterExtension {
    public static double BytesToGB(this ulong bytes) {
        return Math.Round(bytes / ( 1024.0 * 1024.0 * 1024.0 ), 2);
    }
}
