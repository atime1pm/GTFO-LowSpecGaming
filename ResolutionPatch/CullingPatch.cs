using CullingSystem;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.ResolutionPatch
{
    [HarmonyPatch]
    internal class CullingPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_CullingCluster), nameof(C_CullingCluster.Hide))]
        public static bool BuildItDOWN(C_CullingCluster __instance)
        {
            if (__instance.IsShown)
                return false;
            __instance.IsShown = true;
            foreach (Renderer r in __instance.Renderers)
            {
                r.enabled = true;
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_CullingCluster), nameof(C_CullingCluster.Hide))]
        public static bool BuildItUP(C_CullingCluster __instance)
        {
            if (!__instance.IsShown)
                return false;
            __instance.IsShown = false;
            foreach (Renderer r in __instance.Renderers)
            {
                r.enabled = false;
            }
            return false;
        }
    }
}
