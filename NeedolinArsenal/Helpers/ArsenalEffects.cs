using DanielSteginkUtils.Helpers;
using DanielSteginkUtils.Utilities;
using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NeedolinArsenal.Helpers
{
    internal static class ArsenalEffects
    {
        #region Variables
        /// <summary>
        /// Tracks if the coroutine has started
        /// </summary>
        internal static bool isArsenalActive = false;

        /// <summary>
        /// Tracks if the coroutine should continue
        /// </summary>
        internal static bool continueArsenal = false;

        /// <summary>
        /// Stores the Voltvessel beam prefab for ease of reference
        /// </summary>
        private static GameObject? beamPrefab;

        /// <summary>
        /// Stores the Pinstress X-strike prefab for ease of reference
        /// </summary>
        internal static GameObject? needlePrefab;
        #endregion

        /// <summary>
        /// Starts the arsenal, if it isn't already active
        /// </summary>
        internal static void StartArsenal()
        {
            if (!isArsenalActive)
            {
                isArsenalActive = true;
                continueArsenal = true;
                GameManager.instance.StartCoroutine(Arsenal());
            }
        }

        /// <summary>
        /// Runs a loop that triggers the effects of the chosen tool while the needolin is being played
        /// </summary>
        /// <returns></returns>
        private static IEnumerator Arsenal()
        {
            Stopwatch timer = Stopwatch.StartNew();
            while (continueArsenal)
            {
                try
                {
                    if (timer.ElapsedMilliseconds >= 1000)
                    {
                        switch (MusicToolHelper.chosenTool)
                        {
                            // Plasmium Phial - Give 1 lifeblood
                            case MusicTool.Lifeblood:
                                GainLifeblood();
                                break;
                            // Voltvessel - Spawn a volt beam at 1 of 5 locations
                            case MusicTool.Voltvessel:
                                SpawnVoltvesselBeam();
                                break;
                            // Wispfire Lantern - Spawn a wisp for a nearby enemy
                            case MusicTool.Lantern:
                                SpawnWisp();
                                break;
                            // Pin Badge - Produce a needle strike effect to damage nearby enemies
                            case MusicTool.PinBadge:
                                SpawnNeedleStrike();
                                break;
                            default: // Null
                                break;
                        }

                        timer = Stopwatch.StartNew();
                    }
                }
                catch (Exception ex)
                {
                    NeedolinArsenal.Instance.Log($"Error triggering effect: {ex.Message}\n{ex.StackTrace}");
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }

            //NeedolinArsenal.Instance.Log("Stopping effects loop");
            isArsenalActive = false;
        }

        #region Tool Effects
        /// <summary>
        /// Plasmium Phial coats the Needolin's Silk with Plasmium, allowing Hornet to gain Lifeblood masks
        /// </summary>
        private static void GainLifeblood()
        {
            EventRegister.SendEvent(EventRegisterEvents.AddBlueHealth);
        }

        /// <summary>
        /// Voltvessel electrifies the Needolin's Silk, causing it to spawn Voltvessel beams
        /// </summary>
        private static void SpawnVoltvesselBeam()
        {
            if (beamPrefab == null)
            {
                // First get Lightning Rod from the Tools FSM
                PlayMakerFSM toolFsm = HeroController.instance.toolsFSM;
                FsmState voltvesselState = toolFsm.Fsm.GetState("LR Throw");
                SpawnProjectileV2 action = (SpawnProjectileV2)voltvesselState.Actions[4];
                GameObject lightningRod = action.Prefab.Value;

                // Then get the lightning beam from the rod's FSM
                PlayMakerFSM rodFsm = lightningRod.LocateMyFSM("Control");
                FsmState explodeState = rodFsm.Fsm.GetState("Explode");
                SpawnObjectFromGlobalPool action2 = (SpawnObjectFromGlobalPool)explodeState.Actions[0];
                beamPrefab = action2.gameObject.Value;
            }

            // Randomly spawn the beam in 1 of 5 locations
            int random = UnityEngine.Random.RandomRangeInt(-2, 3);
            Transform hcTransform = HeroController.instance.transform;
            Vector3 position = new Vector3()
            {
                x = hcTransform.position.x + (random * 5), // -10, -5, 0, 5, 10
                y = hcTransform.position.y,
                z = hcTransform.position.z
            };
            beamPrefab.Spawn(position);
        }

        /// <summary>
        /// Wispfire Lantern sets the Needolin's Silk alight, creating additional Wisps
        /// </summary>
        private static void SpawnWisp()
        {
            List<GameObject> nearbyEnemies = GetEnemy.GetEnemies(20f);
            if (nearbyEnemies.Count > 0)
            {
                // Get the Wisp prefab
                HeroWispLantern wispLantern = UnityEngine.Object.FindAnyObjectByType<HeroWispLantern>();
                GameObject prefab = ClassIntegrations.GetField<HeroWispLantern, GameObject>(wispLantern, "wispPrefab");

                // Make a duplicate of the Wisp and set it to target nearby enemy
                GameObject wisp = prefab.Spawn(HeroController.instance.transform);
                PlayMakerFSM fsm = wisp.GetComponent<PlayMakerFSM>();
                fsm.FsmVariables.FindFsmGameObject("Target").Value = nearbyEnemies.GetRandomElement();
                fsm.FsmVariables.FindFsmGameObject("Spawner").Value = wispLantern.gameObject;
            }
        }

        /// <summary>
        /// Pin Badge sharpens Hornet's Silk, turning the loose strands into blades that damage nearby enemies
        /// </summary>
        private static void SpawnNeedleStrike()
        {
            List<GameObject> nearbyEnemies = GetEnemy.GetEnemies(20f);
            if (nearbyEnemies.Count > 0)
            {
                // Spawn Cross Slash on random enemy's location
                Transform transform = nearbyEnemies.GetRandomElement().transform;
                Vector3 position = new Vector3()
                {
                    x = transform.position.x,
                    y = transform.position.y,
                    z = transform.position.z
                };
                GameObject tempNeedle = needlePrefab.Spawn(position);

                // Edit the new needle's damagers so they hit enemies, not the player
                GameObject damager1 = tempNeedle.transform.Find("Damager1").gameObject;
                damager1.layer = (int)PhysLayers.HERO_ATTACK;
                damager1.RemoveComponent<DamageHero>();
                damager1.AddComponent<NeedleStrikeDamage>();

                GameObject damager2 = tempNeedle.transform.Find("Damager2").gameObject;
                damager2.layer = (int)PhysLayers.HERO_ATTACK;
                damager2.RemoveComponent<DamageHero>();
                damager2.AddComponent<NeedleStrikeDamage>();
            }
        }
        #endregion
    }
}