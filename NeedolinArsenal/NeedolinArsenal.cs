using BepInEx;
using DanielSteginkUtils.ExternalFiles;
using HarmonyLib;
using NeedolinArsenal.Helpers;

namespace NeedolinArsenal;

[BepInAutoPlugin(id: "io.github.danielstegink.needolinarsenal")]
[BepInDependency("io.github.danielstegink.customneedolin", BepInDependency.DependencyFlags.SoftDependency)]
public partial class NeedolinArsenal : BaseUnityPlugin
{
    internal static NeedolinArsenal Instance { get; private set; }

    private void Awake()
    {
        // Put your initialization logic here
        Instance = this;

        Harmony harmony = new Harmony(Id);
        harmony.PatchAll();

        // Load music files as AudioClips
        MusicToolHelper.shimasenClip = GetAudioClip.GetAudioClipFromAssembly("NeedolinArsenal", "NeedolinArsenal.Resources.Sakura (Zambolino).wav");
        MusicToolHelper.countryClip = GetAudioClip.GetAudioClipFromAssembly("NeedolinArsenal", "NeedolinArsenal.Resources.Desert West (Dagored).wav");
        MusicToolHelper.jazzClip = GetAudioClip.GetAudioClipFromAssembly("NeedolinArsenal", "NeedolinArsenal.Resources.At Ease (Hazelwood).wav");
        MusicToolHelper.metalClip = GetAudioClip.GetAudioClipFromAssembly("NeedolinArsenal", "NeedolinArsenal.Resources.Leader (Zambolino).wav");
        if (MusicToolHelper.shimasenClip == null)
        {
            Log("Audio clips not loaded successfully");
        }

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }

    /// <summary>
    /// Shared logger for the mod
    /// </summary>
    /// <param name="message"></param>
    internal void Log(string message)
    {
        Logger.LogInfo(message);
    }
}