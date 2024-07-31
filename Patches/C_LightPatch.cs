using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CullingSystem;
using HarmonyLib;
using Il2CppSystem.Linq;
using LevelGeneration;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class C_LightPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(C_Light), nameof(C_Light.Setup))]
        public static void GetLightPatch(C_Light __instance) 
        {
            C_CullingClusterPatch.lightList ??= new();
            var light = __instance.TryCast<C_LightDynamic>();
            if (light == null)
            {
                C_CullingClusterPatch.lightList.Add(__instance);
            }
            else if (light.IsFPSPlayerLight)
            { 
                C_CullingClusterPatch.c_LightDynamics.Add(light);
            }
            else
            {
                C_CullingClusterPatch.c_LightDynamics.Add(light);
            }
        }
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(C_LightDynamic), nameof(C_LightDynamic.SetOn))]
        public static void DynamicLightPatch(C_LightDynamic __instance, ref bool mode)
        {

        }

        public static void GetLightPatch2(C_Light __instance)
        {
            C_CullingClusterPatch.lightList ??= new();
            C_CullingClusterPatch.lightList.Add(__instance);
        }
    }
}
