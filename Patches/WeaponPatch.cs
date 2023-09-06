using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LowSpecGaming.Misc
{
    [HarmonyPatch]
    internal class WeaponPatch
    {
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShellCasing), nameof(ShellCasing.Awake))]
        static bool MyMags(ShellCasing __instance)
        {
            __instance.gameObject.active = false;
            GameObject.Destroy(__instance.transform.GetChild(0).gameObject);
            return false;
        }
        [HarmonyPrefix][HarmonyPatch(typeof(DisableAfterDelay), nameof(DisableAfterDelay.Awake))]
        static void MyBullets(DisableAfterDelay __instance) => __instance.delay = 0;
    }
}
