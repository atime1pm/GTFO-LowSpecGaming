using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Il2CppSystem.Collections;
using Decals;
using GameData;
using Il2CppInterop.Runtime.Injection;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.RuntimeDetour;
using AK;

namespace LowSpecGaming.Misc
{
    [HarmonyPatch]
    internal static class SpitterPatch
    {

        //Lowers the quality of spitters
        //
        public static bool hate = true;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InfectionSpitter), nameof(InfectionSpitter.Awake))]
        public static void ShutUpSpitter(InfectionSpitter __instance)
        {
            if (!hate) return;

            Material mat = __instance.m_renderer.material;
            mat.GetTexture(1).mipMapBias = 10;
            mat.GetTexture("_MetallicGlossMap").mipMapBias = 10;
            mat.GetTexture("_SSSMap").mipMapBias = 10;
            mat.GetTexture("_MainTex_B").mipMapBias = 10;
            GameObject.Destroy(__instance.gameObject.GetComponentInChildren<Decal>());
        }
            

        //These 2 stop the spitters from purring
        //
        [HarmonyPrefix][HarmonyPatch(typeof(InfectionSpitter), nameof(InfectionSpitter.StopPurring))]
        public static void StopPurring(InfectionSpitter __instance) => __instance.m_purrLoopPlaying = true;
        [HarmonyPrefix][HarmonyPatch(typeof(InfectionSpitter), nameof(InfectionSpitter.TryPlaySound))]
        public static bool StopPurringSeriously(ref uint id)
        {
            if (id == EVENTS.INFECTION_SPITTER_PURR_LOOP)
                return false;
            return true;
        }

    }
}

