namespace BadMC_Launcher.Models.Data;
public static class AppDataPath {
    public static readonly string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BadMC_Launcher");

    public static readonly string ConfigsPath = Path.Combine(DataPath, "Configs");

    public static readonly string LogsPath = Path.Combine(DataPath, "Logs");

    public static readonly string AssetsPath = Path.Combine(DataPath, "Assets");

    public static readonly string VersionPath = Path.Combine(DataPath, "LauncherConfigs");
}
