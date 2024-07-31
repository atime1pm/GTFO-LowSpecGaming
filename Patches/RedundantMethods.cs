using Agents;
using ChainedPuzzles;
using CullingSystem;
using Enemies;
using FX_EffectSystem;
using HarmonyLib;
using ShaderValueAnimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class RedundantMethods
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUpdateManager), "GamestateUpdate")]
        [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.Update))]
        [HarmonyPatch(typeof(CL_Light), nameof(CL_Light.UpdateData))]
        [HarmonyPatch(typeof(CP_Bioscan_Graphics), nameof(CP_Bioscan_Graphics.Update))]
        [HarmonyPatch(typeof(C_Node), nameof(C_Node.ProcessBuckets))]
        [HarmonyPatch(typeof(C_MovingCuller_Cullbucket), nameof(C_MovingCuller_Cullbucket.Hide))]
        public static IEnumerable<CodeInstruction> DisableTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>();
            code.Clear();
            return code.AsEnumerable();
        }
    }
}
