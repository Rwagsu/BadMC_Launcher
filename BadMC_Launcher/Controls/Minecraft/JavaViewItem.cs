using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Controls.Minecraft;
public partial class JavaViewItem : ObservableObject {
    public JavaViewItem(JavaEntry javaEntry) {
        Entry = javaEntry;

        JavaId = $"{Entry.JavaType} {Entry.MajorVersion}";

        var is64Bit = Entry.Is64bit ? "64Bit" : "32Bit";
        JavaType = $"{is64Bit} / {Entry.JavaVersion}";

        JavaPath = Entry.JavaPath;

        SetIconPath();
    }

    public JavaEntry Entry { get; init; }

    public string JavaId { get; init; }

    public string JavaType { get; init; }

    public string JavaPath { get; init; }

    [ObservableProperty]
    public partial string? JavaIconPath { get; private set; }

    private async void SetIconPath() {
        JavaIconPath = await Entry.GetJavaIconPathAsync();
    }

    public static bool operator ==(JavaViewItem? left, JavaViewItem? right) {
        var leftDirectoryPath = Path.GetDirectoryName(left?.JavaPath);
        var rightDirectoryPath = Path.GetDirectoryName(right?.JavaPath);

        return left is not null && right is not null
            ? leftDirectoryPath == rightDirectoryPath
            : ReferenceEquals(left, right);
    }

    public static bool operator !=(JavaViewItem? left, JavaViewItem? right) {
        var leftDirectoryPath = Path.GetDirectoryName(left?.JavaPath);
        var rightDirectoryPath = Path.GetDirectoryName(right?.JavaPath);

        return left is not null && right is not null
            ? leftDirectoryPath != right.JavaPath
            : !ReferenceEquals(left, right);
    }

    public override bool Equals(object? obj) {
        if (obj is JavaViewItem viewItem) {
            if (ReferenceEquals(this.JavaPath, viewItem.JavaPath)) {
                return true;
            }

            if (!ReferenceEquals(this.JavaPath, viewItem.JavaPath)) {
                return false;
            }
        }
        throw new NotImplementedException();
    }

    public override int GetHashCode() {
        // HashCode.Combine generates a hash code by combining the hash codes of the provided fields.
        // This ensures that the hash code is unique based on the values of these fields.
        return HashCode.Combine(JavaPath);
    }
}
