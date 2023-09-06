using CullingSystem;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class CullingPatch
    {
        //Taking off Null check and use foreach loop instead of a for loop for better performance
        //maybe....
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_CullingCluster), nameof(C_CullingCluster.Show))]
        public static bool BuildItDOWN(C_CullingCluster __instance)
        {
            if (__instance.IsShown) return false;

            __instance.IsShown = true;
            
            foreach (Renderer r in __instance.Renderers)
                r.enabled = true;
            
            C_CullingManager.Register((C_Cullable)__instance);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_CullingCluster), nameof(C_CullingCluster.Hide))]
        public static bool BuildItUP(C_CullingCluster __instance)
        {
            if (!__instance.IsShown) return false;

            __instance.IsShown = false;
            
            foreach (Renderer r in __instance.Renderers)
                r.enabled = false;
            
            return false;
        }
    }
}
