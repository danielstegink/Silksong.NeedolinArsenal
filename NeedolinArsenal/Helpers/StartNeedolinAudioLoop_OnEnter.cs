using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using System.Collections.Generic;
using UnityEngine;
using DanielSteginkUtils.Helpers;

namespace NeedolinArsenal.Helpers
{
    [HarmonyPatch(typeof(StartNeedolinAudioLoop), "OnEnter")]
    public static class StartNeedolinAudioLoop_OnEnter
    {
        [HarmonyPrefix]
        public static void Prefix(StartNeedolinAudioLoop __instance)
        {
            // Verify we have the right state
            // Normally I would check the FSM name, but apparently its just "FSM"
            if (!__instance.State.Name.Equals("Start Needolin Proper"))
            {
                return;
            }

            //NeedolinArsenal.Instance.Log($"Default clip: {Needolin.DefaultClip.name}");

            // Get a list of equipped music tools
            List<string> equippedTools = GetTools.GetEquippedTools();
            List<MusicTool> musicTools = MusicToolHelper.GetMusicTools(equippedTools);

            // If none of them are equipped, reset to default
            if (musicTools.Count == 0)
            {
                if (Needolin.NeedolinFsm != null && 
                    Needolin.DefaultClip != null)
                {
                    NeedolinArsenal.Instance.Log("No tools equipped; resetting AudioClip");
                    MusicToolHelper.chosenTool = null;
                    Needolin.ResetAudioClip(Needolin.NeedolinFsm, true);

                    //NeedolinArsenal.Instance.Log($"Default clip: {Needolin.DefaultClip.name}");
                    //NeedolinArsenal.Instance.Log($"New default clip: {Needolin.GetDefaultClip(true).name}");
                    //NeedolinArsenal.Instance.Log($"Current clip: {Needolin.CurrentClip.name}");
                }
                
                return;
            }

            // If we haven't selected a tool, pick 1 at random
            if (!ArsenalEffects.isArsenalActive)
            {
                MusicToolHelper.chosenTool = musicTools.GetRandomElement();
            }

            // Replace the music clip; important we do this each time in case another mod has reset the audio clip
            //NeedolinArsenal.Instance.Log("Replacing AudioClip");
            AudioClip? newClip = MusicToolHelper.GetAudioClip(MusicToolHelper.chosenTool);
            if (newClip != null)
            {
                Needolin.SetNewAudioClip(__instance.Fsm, newClip, true);

                //NeedolinArsenal.Instance.Log($"Default clip: {Needolin.DefaultClip.name}");
                //NeedolinArsenal.Instance.Log($"New default clip: {Needolin.GetDefaultClip(true).name}");
                //NeedolinArsenal.Instance.Log($"Current clip: {Needolin.CurrentClip.name}");
            }

            // Trigger the arsenal if it isn't already active
            ArsenalEffects.StartArsenal();
        }
    }
}