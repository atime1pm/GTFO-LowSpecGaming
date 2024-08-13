using ChainedPuzzles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Misc
{
    [HarmonyPatch]
    internal static class BioScanPatch
    { /// I STOLE THIS FROM MCCAD PLEASE DONT SUEEE MEEEEEEjioajkofkolasdflasjnldf

        public static bool update = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CP_Holopath_Spline), nameof(CP_Holopath_Spline.Reveal))]
        static void Holopath_Spline_Reveal(CP_Holopath_Spline __instance)
        {
            __instance.CurvySpline.UseThreading = true;
            __instance.CurvySpline.UpdateIn = (FluffyUnderware.Curvy.CurvyUpdateMethod)(-1);
            __instance.CurvyExtrusion.Optimize = true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CP_Bioscan_Graphics), nameof(CP_Bioscan_Graphics.Setup))]
        static void Bioscan_Graphics_Setup(CP_Bioscan_Graphics __instance)
        {
            if (!__instance.m_colorsByMode.ContainsKey(eChainedPuzzleGraphicsColorMode.Alarm_Waiting)) __instance.m_colorsByMode.Add(eChainedPuzzleGraphicsColorMode.Alarm_Waiting, __instance.m_currentCol);
            if (!__instance.m_colorsByMode.ContainsKey(eChainedPuzzleGraphicsColorMode.Alarm_TimedOut)) __instance.m_colorsByMode.Add(eChainedPuzzleGraphicsColorMode.Alarm_TimedOut, __instance.m_colorsByMode[eChainedPuzzleGraphicsColorMode.Alarm_Waiting]);
            if (!__instance.m_colorsByMode.ContainsKey(eChainedPuzzleGraphicsColorMode.Alarm_Active)) __instance.m_colorsByMode.Add(eChainedPuzzleGraphicsColorMode.Alarm_Active, __instance.m_colorsByMode[eChainedPuzzleGraphicsColorMode.Alarm_Waiting] * new Color(1.1f, 1.1f, 1.1f, 1.1f));

            __instance.m_colorsByMode[eChainedPuzzleGraphicsColorMode.Alarm_Active] *= new Color(1.1f, 1.1f, 1.1f, 1.1f);
        }
    }
}
