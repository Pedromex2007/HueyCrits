using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using HueyCrits.Windows;
using System.Numerics;
using System;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Gui.FlyText;
using System.Threading.Tasks;
using ImGuiScene;

namespace HueyCrits;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IFlyTextGui FlyTextGui { get; private set; } = null!;

    private const string CommandName = "/testsample";
    private string pluginDirectory;

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("HueyCrits");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private HueyWindow HueyWindow { get; init; }

    public Plugin()
    {

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // you might normally want to embed resources and load them from the manifest stream
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "fakegoat.png");
        var hueyImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "huey1.png");
        pluginDirectory = PluginInterface.AssemblyLocation.Directory?.FullName!;


        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);
        HueyWindow = new HueyWindow(this, pluginDirectory);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.RemoveWindow(ConfigWindow);
        //WindowSystem.AddWindow(HueyWindow);

        //ChatGui.ChatMessage += TestBop;
        FlyTextGui.FlyTextCreated += CriticalFlytextAchieved;

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        //CommandManager.AddHandler("/huey", new CommandInfo(OnHueyCommand) {
        //    HelpMessage = hueyImagePath
        //});

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    private void CriticalFlytextAchieved(ref FlyTextKind kind, ref int val1, ref int val2, 
        ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (kind == FlyTextKind.DamageCritDh || kind == FlyTextKind.DamageCrit || kind == FlyTextKind.AutoAttackOrDot)
        {
            //ChatGui.Print("Autoattack Huey POP!");

            WindowSystem.AddWindow(HueyWindow);
            HueyWindow.lerpFloat = 1;

            HueyWindow.DecrementOpacity();
            HueyWindow.SelectHueyPicture();

            HueyWindow.IsOpen = true;
            DestroyThisWindow();

        }
    }

    private async void DestroyThisWindow()
    {
        await Task.Delay(1000);
        WindowSystem.RemoveWindow(HueyWindow);
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();
        HueyWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnHueyCommand(string command, string args)
    {
        //HueyWindow.SelectHueyPicture();
        //HueyWindow.Toggle();
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
