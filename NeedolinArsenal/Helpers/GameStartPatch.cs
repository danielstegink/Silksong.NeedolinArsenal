using HarmonyLib;
using System;
using UnityEngine;
using DanielSteginkUtils.ExternalFiles;

namespace NeedolinArsenal.Helpers
{
    [HarmonyPatch(typeof(GameManager), "Awake")]
    public static class GameStartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(GameManager __instance)
        {
            GetCrossSlash();
        }

        /// <summary>
        /// Gets the Pinstress' Cross Slash ability from asset bundles so we can use it in the arsenal
        /// </summary>
        private static void GetCrossSlash()
        {
            // Get the Cross Slash object from the Asset Bundle indicated by the Pinstress' FSM
            AssetBundle? bundle = GetAssetBundle.GetBundle("localpoolprefabs_assets_areaswamp.bundle");
            if (bundle == null)
            {
                NeedolinArsenal.Instance.Log("Error loading bundle");
                return;
            }

            try
            {
                GameObject prefab = (GameObject)bundle.LoadAsset("Assets/Prefabs/Hornet Enemies/Pinstress CrossSlash.prefab");
                if (prefab == null)
                {
                    NeedolinArsenal.Instance.Log("Cross Slash prefab not found");
                    return;
                }

                // Then fully customize it
                ArsenalEffects.needlePrefab = UnityEngine.GameObject.Instantiate(prefab);
                ArsenalEffects.needlePrefab.name = "NeedolinArsenal.NeedleStrike";
                ArsenalEffects.needlePrefab.SetActive(false);
                UnityEngine.GameObject.DontDestroyOnLoad(ArsenalEffects.needlePrefab);
            }
            catch (Exception ex)
            {
                NeedolinArsenal.Instance.Log($"Error getting Cross Slash prefab: {ex.Message}\n\n{ex.StackTrace}");
            }
            finally
            {
                bundle.Unload(true);
            }
        }
    }
}