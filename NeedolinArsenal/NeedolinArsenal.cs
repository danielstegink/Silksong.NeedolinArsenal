using BepInEx;
using DanielSteginkUtils.ExternalFiles;
using HarmonyLib;
using NeedolinArsenal.Helpers;
using Silksong.AssetHelper.ManagedAssets;
using System.Reflection;
using UnityEngine;

namespace NeedolinArsenal;

[BepInAutoPlugin(id: "io.github.danielstegink.needolinarsenal")]
[BepInDependency("io.github.danielstegink.customneedolin", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("org.silksong-modding.assethelper")]
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
        Assembly assembly = Assembly.GetExecutingAssembly();
        MusicToolHelper.shimasenClip = GetAudioClip.GetAudioClipFromAssembly(assembly, "NeedolinArsenal.Resources.Sakura (Zambolino).wav");
        MusicToolHelper.countryClip = GetAudioClip.GetAudioClipFromAssembly(assembly, "NeedolinArsenal.Resources.Desert West (Dagored).wav");
        MusicToolHelper.jazzClip = GetAudioClip.GetAudioClipFromAssembly(assembly, "NeedolinArsenal.Resources.At Ease (Hazelwood).wav");
        MusicToolHelper.metalClip = GetAudioClip.GetAudioClipFromAssembly(assembly, "NeedolinArsenal.Resources.Leader (Zambolino).wav");
        if (MusicToolHelper.shimasenClip == null)
        {
            Log("Audio clips not loaded successfully");
        }

        ArsenalEffects.needlePrefab = ManagedAsset<GameObject>.FromNonSceneAsset("Assets/Prefabs/Hornet Enemies/Pinstress CrossSlash.prefab", 
                                                                                    "localpoolprefabs_assets_areaswamp.bundle");
        ArsenalEffects.trobbioPrefab = ManagedAsset<GameObject>.FromNonSceneAsset("Assets/Prefabs/Heroes/Effects/hero_dazzle_flash.prefab",
                                                                                    "localpoolprefabs_assets_shared.bundle");
        MusicToolHelper.trobbioAsset = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/HornetMusic/Battle/H144-71 WIP Trobbio.wav",
                                                                                    "sfxstatic_assets_trobbio.bundle");

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