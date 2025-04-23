using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Services.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;
using Uno.Extensions;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using WinRT.Interop;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;

public partial class JavaContentDialogViewModel : ObservableObject {
    private readonly MinecraftConfigService minecraftConfigService;

    public JavaContentDialogViewModel() {
        minecraftConfigService = App.GetService<MinecraftConfigService>();

        minecraftConfigService.PropertyChanged += JavaList_PropertyChanged;

        IsAutoJavaEnabled = minecraftConfigService.IsAutoJavaEnabled;

        // Initialize List
        JavasList = new();
        SetJavaList();
    }

    [ObservableProperty]
    public partial bool IsJavasListEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsJavasListLoading { get; set; }

    [ObservableProperty]
    public partial bool IsAutoJavaEnabled { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<JavaViewItem> JavasList { get; set; }

    [ObservableProperty]
    public partial int JavasListSelectedIndex { get; set; }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task AddJava() {
        // Create new file picker
        var filePicker = new FileOpenPicker();

        // Get window handle
        var hwnd = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(filePicker, hwnd);

        if (OperatingSystem.IsWindows()) {
            filePicker.FileTypeFilter.Add(".exe");
        }

        // Show file picker dialog
        var file = await filePicker.PickSingleFileAsync();

        if (file != null && file.DisplayName == "java") {
            minecraftConfigService.JavaPaths.Add(file.Path);
            return;
        }
        else if(file != null && file.DisplayName == "javaw") {
            var javaFolder = Directory.GetParent(file.Path);

            if (javaFolder != null) {
                if (OperatingSystem.IsWindows()) {
                    minecraftConfigService.JavaPaths.Add(@$"{javaFolder.FullName}\java{file.FileType}");
                    return;
                }

                minecraftConfigService.JavaPaths.Add($"{javaFolder.FullName}/java{file.FileType}");
                return;
            }
        }

        // TODO: Show tip toast
    }

    [RelayCommand]
    private async Task SearchJavas() {
        IAsyncEnumerable<JavaEntry>? javas = JavaUtil.EnumerableJavaAsync();
        if (javas != null) {
            var javaList = await Task.Run(async () => {
                var list = new List<string>();
                await foreach (var item in javas) {
                    list.Add(item.JavaPath);
                }
                return list;
            });

            minecraftConfigService.JavaPaths.MargeItems(javaList);
            return;
        }

        //TODO: Show tip toast
    }

    [RelayCommand]
    private void SetActiveJava() {
        // Get selected item
        var selectedItem = JavasList.ElementAtOrDefault(JavasListSelectedIndex);

        // Set active Java path
        minecraftConfigService.ActiveJavaPath = minecraftConfigService.JavaPaths.FirstOrDefault(item => item == selectedItem?.JavaPath);
    }

    [RelayCommand]
    private void ChangeIsAutoJavaEnabled() {
        minecraftConfigService.IsAutoJavaEnabled = IsAutoJavaEnabled;
    }

    [RelayCommand]
    private void DeleteJava(string parameter) {
        // Check if parameter is null or empty
        if (minecraftConfigService.JavaPaths.Contains(parameter)) {
            // Remove Java path
            minecraftConfigService.JavaPaths.Remove(parameter);
            return;
        }
        // TODO: Toast Tips
    }

    [RelayCommand]
    private void LocalViewJava(string parameter) {
        App.GetService<FileService>().TryOpenFolderFromPath(parameter);
    }

    partial void OnJavasListChanged(ObservableDataList<JavaViewItem> value) {
        IsJavasListEmpty = !JavasList.Any();
    }

    private async void SetJavaList() {
        IsJavasListLoading = true;

        var javaEntries = await Task.Run(async () => {
            // Get Java paths
            var tasks = minecraftConfigService.JavaPaths
                .Select(async item => await JavaUtil.GetJavaInfoAsync(item)).ToList();

            // Convert list
            return await Task.WhenAll(tasks);
        });

        // Clear list
        JavasList.Clear();

        // Add items
        foreach (var item in javaEntries) {
            JavasList.Add(new JavaViewItem(item));
        }

        IsJavasListEmpty = !JavasList.Any();

        // Find selected Java
        JavasListSelectedIndex = JavasList.GetIndex(item => item.JavaPath == minecraftConfigService.ActiveJavaPath);

        IsJavasListLoading = false;
    }

    private void JavaList_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        switch(e.PropertyName) {
            case nameof(MinecraftConfigService.JavaPaths):
                // Update Java list
                SetJavaList();
                break;
            case nameof(MinecraftConfigService.ActiveJavaPath):
                // Update selected index
                // TODO: Debug
                JavasListSelectedIndex = JavasList.GetIndex(item => item.JavaPath == minecraftConfigService.ActiveJavaPath);
                break;
        }
        IsJavasListEmpty = !JavasList.Any();
    }
}
