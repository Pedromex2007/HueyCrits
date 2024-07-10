using Dalamud.Interface.Windowing;
using System.Numerics;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace HueyCrits.Windows
{
    public class HueyWindow : Window, IDisposable
    {
        public int ImgNum { get; set; } = 1;
        public float lerpFloat { get; set; } = 1f;
        private string imgHueyPath;
        public HueyWindow(Plugin plugin, string imgPath) : base("Huey Window. Nothing else.")
        {
            Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoMouseInputs;

            Position = new Vector2(GetGameResolution().Item1 / 4, 0);
            Size = new Vector2(7000, 7000);
            //imgHueyPath = imgPath;
            imgHueyPath = Path.Combine(Plugin.PluginInterface.AssemblyLocation.Directory?.FullName!, "huey1.png");

        }

        public unsafe (int, int) GetGameResolution()
        {

            var frameInstance = Framework.Instance();
            return (frameInstance->GameWindow->WindowWidth, frameInstance->GameWindow->WindowHeight);

        }

        public void Dispose() {}

        public async void DecrementOpacity()
        {
            while (lerpFloat > 0)
            {
                await System.Threading.Tasks.Task.Delay(10);
                lerpFloat -= 0.01f;
            }
            //await Task.Delay(1);

        }

        /// <summary>
        /// Select a random picture of Huey Long. Name must be "huey" followed with a number.
        /// </summary>
        public void SelectHueyPicture()
        {
            Random r = new Random();
            int rInt = r.Next(1, 3);
            ImgNum = rInt;
            imgHueyPath = Path.Combine(Plugin.PluginInterface.AssemblyLocation.Directory?.FullName!, "huey" + ImgNum + ".png");
        }


        public override void Draw()
        {
            var hueyImg = Plugin.TextureProvider.GetFromFile(imgHueyPath).GetWrapOrDefault();

            if (hueyImg != null)
            {
                ImGui.Image(hueyImg.ImGuiHandle, new Vector2(1500, 1500), default, Vector2.One, new Vector4(1,1,1,lerpFloat));
            } else
            {
                ImGui.Text("It's Fucked you fucking dumbass bitch cunt.");
            }

        }
    }
}
