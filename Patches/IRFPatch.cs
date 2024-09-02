using HarmonyLib;
using IRF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class IRFPatch
    {

        public static bool draw;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InstancedRenderFeature), nameof(InstancedRenderFeature.OnEnable))]
        public static void InstancedRenderFeaturePatch(InstancedRenderFeature __instance)
        {
            if (draw) return;
            try
            {
                if (!__instance.m_descriptor.name.ToLower().Contains("tentacle") && draw)
                {
                    __instance.enabled = false;
                    GameObject.Destroy(__instance.gameObject);
                }
            }
            catch { }
        }
    }
}
