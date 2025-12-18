using DanielSteginkUtils.Helpers;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using NeedolinArsenal.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace ScavengerOfPharloom.Helpers
{
    [HarmonyPatch(typeof(StartNeedolinAudioLoop), "OnEnter")]
    public static class StartNeedolinAudioLoopPatch
    {
        /// <summary>
        /// Stores the default audio clip for the needolin
        /// </summary>
        internal static AudioClip? defaultClip;

        [HarmonyPrefix]
        public static void Prefix(StartNeedolinAudioLoop __instance)
        {
            // Verify we have the right state
            // Normally I would check the FSM name, but apparently its just "FSM"
            if (!__instance.State.Name.Equals("Start Needolin Proper"))
            {
                return;
            }

            // Store default audio clip
            NeedolinArsenal.NeedolinArsenal.Instance.Log($"Playing audio clip: {__instance.DefaultClip.Value.name}");
            if (defaultClip == null)
            {
                defaultClip = (AudioClip)__instance.DefaultClip.Value;
            }

            // Get a list of equipped music tools
            List<string> equippedTools = GetTools.GetEquippedTools();
            List<MusicTool> musicTools = MusicToolHelper.GetMusicTools(equippedTools);

            // If none of them are equipped, reset to default
            if (musicTools.Count == 0)
            {
                NeedolinArsenal.NeedolinArsenal.Instance.Log("No tools equipped; resetting AudioClip");
                MusicToolHelper.chosenTool = null;
                __instance.DefaultClip.Value = defaultClip;
                return;
            }

            // If we've already chosen a tool, we don't need to select a new one
            if (ArsenalEffects.continueArsenal)
            {
                NeedolinArsenal.NeedolinArsenal.Instance.Log("AudioClip already replaced");
                return;
            }

            // Choose one at random
            MusicToolHelper.chosenTool = musicTools.GetRandomElement();

            // Replace the music clip
            NeedolinArsenal.NeedolinArsenal.Instance.Log("Replacing AudioClip");
            __instance.DefaultClip.Value = MusicToolHelper.GetAudioClip(MusicToolHelper.chosenTool);

            // Trigger the arsenal if it isn't already active
            ArsenalEffects.StartArsenal();
        }
    }
}