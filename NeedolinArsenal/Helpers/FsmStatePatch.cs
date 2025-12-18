using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ScavengerOfPharloom.Helpers;
using System.Collections.Generic;

namespace NeedolinArsenal.Helpers
{
    [HarmonyPatch(typeof(FsmState), "OnEnter")]
    public static class FsmStatePatch
    {
        [HarmonyPostfix]
        public static void Postfix(FsmState __instance)
        {
            //if (__instance.Fsm.Name.Equals("FSM"))
            //{
            //    NeedolinArsenal.Instance.Log($"Entering FSM state {__instance.Name}");
            //}

            // Make sure we're in the correct state and
            // that there is reason to believe the needolin must reset
            if (StartNeedolinAudioLoopPatch.defaultClip != null &&
                __instance.Name.Equals("Fast Travel Check"))
            {
                // Verify the target state exists, just in case there's another FSM with a Fast Travel Check state
                FsmState state = __instance.Fsm.GetState("Start Needolin Proper");
                if (state != null)
                {
                    //NeedolinArsenal.Instance.Log("Resetting AudioClip from start of chain");
                    StartNeedolinAudioLoop action = (StartNeedolinAudioLoop)state.Actions[6];
                    action.DefaultClip.Value = StartNeedolinAudioLoopPatch.defaultClip;
                }
            }

            // If we stop playing the regular Needolin, stop the arsenal
            if (StopArsenal(__instance.Name))
            {
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
    }
}