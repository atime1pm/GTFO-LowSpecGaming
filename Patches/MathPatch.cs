using CullingSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Enemies;
using IRF;
using Newtonsoft.Json.Linq;
using CellMenu;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class MathPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MathUtil), nameof(MathUtil.RotateVector2))]
        public static bool RotateVector2Patch(ref Vector2 v, ref float angleDeg,ref Vector2 __result )
        {
            float f = 3.141592f / 180f * angleDeg;
            float num1 = MathApprox.Cos(f);
            float num2 = MathApprox.Sin(f);
            __result = new Vector2(v.x * num1 - v.y * num2,v.x * num2 + v.y * num1);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SentryScannerPlane), nameof(SentryScannerPlane.Update))]
        public static bool SentryScannerPlaneUpdatePatch(SentryScannerPlane __instance)
        {
            __instance.m_fastDelta = Mathf.Lerp(__instance.m_fastDelta, __instance.FastScan ? 1f : 0.0f, Time.deltaTime * 4f);
            __instance.m_material.SetFloat(SentryScannerPlane._HasTarget, __instance.m_fastDelta);
            return false;
        }
    }
}
