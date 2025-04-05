using System;
using System.Diagnostics;
using System.Net;
using BadMC_Launcher.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Parser;
using MinecraftLaunch.Extensions;
using MinecraftLaunch.Utilities;
using Windows.Gaming.UI;

namespace BadMC_Launcher.Tests;

public class UnitTest1 {
    class C1 {
        public string indowName { get; set; } = "哼哼啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊！！！！！！！！！！！！！！";

        public int indowName2 { get; set; } = 13333444;
    }
        [SetUp]
    public void Setup() {
    }

    [Test]
    public async Task Test1()
    {
        var a = await JavaUtil.GetJavaInfoAsync(@"C:\Program Files\Zulu\zulu-23\bin\java.exe");

        Assert.Pass($"{a.JavaPath}\n{a.JavaType}\n{a.JavaVersion}\n{a.MajorVersion}");
    }
}
