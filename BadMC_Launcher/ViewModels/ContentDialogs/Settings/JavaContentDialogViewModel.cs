using System.ComponentModel;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Services.Settings;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;
using Windows.Storage.Pickers;
using Uno.Extensions.Specialized;
using WinRT.Interop;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;

public partial class JavaContentDialogViewModel : ObservableObject {
    private readonly MinecraftConfigService minecraftConfigService;

    public JavaContentDialogViewModel() {
        minecraftConfigService = App.GetService<MinecraftConfigService>();

        minecraftConfigService.PropertyChanged += JavaList_PropertyChanged;

        IsAutoJavaEnabled = minecraftConfigService.IsAutoJavaEnabled;
        IsJavasListLoading = false;

        // Initialize List
        JavasList = [];
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
        var windowHandle = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(filePicker, windowHandle);

        if (OperatingSystem.IsWindows()) {
            filePicker.FileTypeFilter.Add(".exe");
        }

        // Show file picker dialog
        var file = await filePicker.PickSingleFileAsync();

        switch (file) {
            case { DisplayName: "java" }:
                minecraftConfigService.JavaPaths.Add(file.Path);
                break;
            case { DisplayName: "javaw" }: {
                var javaFolder = Directory.GetParent(file.Path);

                if (javaFolder != null) {
                    if (OperatingSystem.IsWindows()) {
                        minecraftConfigService.JavaPaths.Add(@$"{javaFolder.FullName}\java{file.FileType}");
                        return;
                    }

                    minecraftConfigService.JavaPaths.Add($"{javaFolder.FullName}/java{file.FileType}");
                    return;
                }

                break;
            }
        }

        // TODO: Show tip toast
    }

    [RelayCommand]
    private void DownloadJava(string parameter) {
        
    }

    [RelayCommand]
    private async Task SearchJavas() {
        IAsyncEnumerable<JavaEntry>? javas = JavaUtil.EnumerableJavaAsync();
        if (javas != null) {
            var javaList = await Task.Run(async () => await javas.ToListAsync());

            if (!javaList.All(item => JavasList.Contains(item.JavaPath))) {
                JavasList.MargeItems(javaList.Select(item => new JavaViewItem(item)));
                minecraftConfigService.JavaPaths.MargeItems(javaList.Select(item => item.JavaPath));
                return;
            }
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
        if (!minecraftConfigService.JavaPaths.Contains(parameter)) {
            return;
        }

        
        var deleteItem = JavasList.FirstOrDefault(item => item.JavaPath == parameter);
        if (deleteItem != null) {
            // Remove Java path
            JavasList.Remove(deleteItem);
            minecraftConfigService.JavaPaths.Remove(deleteItem.JavaPath);
            // TODO: Toast Tips
            return;
        }
        
        // TODO: Toast Tips
    }

    [RelayCommand]
    private void LocalViewJava(string parameter) {
        App.GetService<FileService>().TryOpenFolderOrFileFromPath(parameter);
    }

    partial void OnJavasListChanged(ObservableDataList<JavaViewItem> value) {
        IsJavasListEmpty = !JavasList.Any();
    }

    private async void SetJavaList() {
        if (!minecraftConfigService.JavaPaths.Any()) {
            return;
        }

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
                if (!minecraftConfigService.JavaPaths.SequenceEqual(JavasList.Select(item => item.JavaPath))) {
                    // Update Java list
                    SetJavaList();
                }
                break;
            case nameof(MinecraftConfigService.ActiveJavaPath):
                if (minecraftConfigService.ActiveJavaPath != JavasList.ElementAtOrDefault(JavasListSelectedIndex)?.JavaPath) {
                    // Update selected index
                    JavasListSelectedIndex = JavasList.GetIndex(item => item.JavaPath == minecraftConfigService.ActiveJavaPath);
                }
                break;
        }
        IsJavasListEmpty = !JavasList.Any();
    }
}
