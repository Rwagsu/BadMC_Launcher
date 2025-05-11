using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.MainSearch;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.ViewServices;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using BadMC_Launcher.Views.Pages.Settings;
using BadMC_Launcher.Views.Pages;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using Serilog;
using Uno.Resizetizer;

namespace BadMC_Launcher;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    public static new App Current => (App)Application.Current;

    internal Window? MainWindow { get; private set; }
    internal IHost? Host { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true, (configuration) => {
                    configuration
                        .MinimumLevel.Error()
                        .WriteTo.Console()
                        .WriteTo.File(
                            path: Path.Combine(AppDataPath.LogsPath, "AppLog.log"),
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 10
                        );
                })
                .ConfigureServices((context, services) =>
                {
                    // TODO: Register your services
                    //Register third-party class
                    services.AddSingleton<HttpClient>();
                    services.AddSingleton<ResourceLoader>();

                    services.AddTransient<Random>();

                    //Register class
                    services.AddSingleton<ExceptionHandlingService>();
                    services.AddSingleton<FileService>();
                    services.AddSingleton<MinecraftConfigsService>();
                    services.AddSingleton<ThemeConfigsService>();
                    services.AddSingleton<MainSideBarService>();
                    services.AddSingleton<SettingsService>();
                    services.AddSingleton<AppAssetsService>();

                    services.AddTransient<SingleMinecraftConfigsService>();

                    //Register ContentDialogs
                    services.AddTransient<MinecraftFolderContentDialog>();
                    services.AddTransient<JavaContentDialog>();
                })
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = builder.Build();

        // Get Configs
        GetSettings();

        // Register Pages
        GlobalRegister();

        //Set MainWindow Configs
        MainWindow.AppWindow.Title = GetService<ThemeConfigsService>().WindowName;
        MainWindow.AppWindow.Resize(AppParameters.WindowSize);
        MainWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        // TODO: 不等了拖拽三大金刚自己写(恼)
#if WINDOWS
        MainWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        MainWindow.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }
        // Ensure the current window is active
        MainWindow.Activate();
    }

    //Get Service
    public static T GetService<T>() {
        var services = Current.Host?.Services;
        if (services == null) {
            throw new InvalidOperationException("Service not found.");
        }

        var service = services.GetService<T>();
        if (service != null) {
            return service;
        }
        throw new InvalidOperationException("Service not found.");
    }

    private static void GetSettings() {
        GetService<MinecraftConfigsService>().SyncSettingGet();
        GetService<MinecraftConfigsService>().IsSyncEnabled = true;
        GetService<ThemeConfigsService>().SyncSettingGet();
        GetService<ThemeConfigsService>().isSyncEnabled = true;
    }

    private void GlobalRegister() {
        //Register MainSideBarItems
        GetService<MainSideBarService>().Register(new MainSideBarItem() {
            ItemName = GetService<ResourceLoader>().GetString("MainPage_SettingsNameResource"),
            ItemIcon = new AnimatedIcon() {
                Source = new AnimatedSettingsVisualSource(),
                FallbackIconSource = new FontIconSource() { Glyph = "\uE713" },
            },
            NavigatePage = typeof(SettingsDashboardPage)
        }, true);

        //Register MainMenuSearchFilterItems
        GetService<MainSideBarService>().SearchFilterRegister(new MainMenuSearchMinecraftEntryFilter() {
            ItemName = GetService<ResourceLoader>().GetString("MainPage_SearchFilterMinecraftEntryNameResource"),
            IconGlyph = "\uE7FC"
        });

        //Register SettingsSideBarItems
        GetService<SettingsService>().SideBarRegister(new SettingsSideBarItem() {
            ItemName = GetService<ResourceLoader>().GetString("LaunchSettingsPage_SettingsPageName"),
            ItemIcon = new FontIcon() { Glyph = "\uE7FC" },
            NavigatePage = typeof(LaunchSettingsPage),
            PageHead = GetService<ResourceLoader>().GetString("LaunchSettingsPage_SettingsPageName"),
        });
    }
}
