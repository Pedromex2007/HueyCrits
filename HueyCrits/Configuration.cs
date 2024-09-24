using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace HueyCrits;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public float GlobalSoundVolume = 1f;
    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
