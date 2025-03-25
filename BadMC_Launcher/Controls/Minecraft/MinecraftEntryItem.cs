using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using CommunityToolkit.WinUI.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BadMC_Launcher.Controls.Minecraft;
public class MinecraftEntryItem {
    public string MinecraftId => MinecraftEntry.Id;

    public required MinecraftEntry MinecraftEntry { get; init; }

    public required BitmapImage MinecraftImage { get; init; }

    public required HashSet<MetadataItem> MinecraftTags { get; init; }

    public bool IsStarred { get; set; }

    public void SetIsStarredEvent(object? sender, NotifyCollectionChangedEventArgs e) {
        if (sender is ObservableDataList<string> folderEntry) {
            IsStarred = folderEntry.IndexOf(MinecraftId) >= 0;
        }
    }
}
