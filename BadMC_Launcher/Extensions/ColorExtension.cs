using Windows.UI;

namespace BadMC_Launcher.Extensions;

public static class ColorExtension {
    public static string ToNoAlphaHex(this Color color) => System.Drawing.ColorTranslator.ToHtml(
        System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
    );
}
