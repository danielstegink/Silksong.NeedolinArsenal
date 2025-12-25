using HarmonyLib;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace NeedolinArsenal.Helpers
{
    [HarmonyPatch(typeof(FsmState), "OnEnter")]
    public static class FsmState_OnEnter
    {
        [HarmonyPostfix]
        public static void Postfix(FsmState __instance)
        {
            // If we stop playing the regular Needolin, stop the arsenal
            if (StopArsenal(__instance.Name))
            {
                //PrintAudioSources();
                ArsenalEffects.continueArsenal = false;
                //NeedolinArsenal.Instance.Log("Ending default needolin");
            }

            // If we start playing the regular Needolin again, restart the arsenal
            if (RestartArsenal(__instance.Name))
            {
                //NeedolinArsenal.Instance.Log("Re-enabling effects loop");
                ArsenalEffects.StartArsenal();
            }
        }

        /// <summary>
        /// Checks whether or not to stop the arsenal
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        private static bool StopArsenal(string stateName)
        {
            List<string> endStateNames = new List<string>()
            {
                // These states indicate that we are stopping the Needolin
                "Break Loop",
                "Cancel Needolin?",
                "Pass CANCEL",
                "End Needolin",
                "Needolin Lock",
                "Needolin Lock 2",

                // These states are for special Needolin sequences where we also shouldn't trigger the arsenal
                "Needolin Mem In",
                "Needolin FT In",
            };

            return endStateNames.Contains(stateName);
        }

        /// <summary>
        /// Checks whether or not to restart the arsenal
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        private static bool RestartArsenal(string stateName)
        {
            List<string> restartStates = new List<string>()
            {
                "Play Needolin",
            };

            return restartStates.Contains(stateName);
        }

        //private static void PrintAudioSources()
        //{
        //    NeedolinArsenal.Instance.Log("All audio sources");
        //    AudioSource[] audioSources = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        //    foreach (AudioSource audioSource in audioSources)
        //    {
        //        string name = audioSource.name;
        //        GameObject? gameObject = audioSource.gameObject;
        //        while (gameObject != null)
        //        {
        //            name = $@"{gameObject.name}\{name}";

        //            Transform parent = gameObject.transform.parent;
        //            if (parent != null)
        //            {
        //                gameObject = gameObject.transform.parent.gameObject;
        //            }
        //            else
        //            {
        //                gameObject = null;
        //            }
        //        }

        //        NeedolinArsenal.Instance.Log($"{name}");
        //    }
        //}
    }
}