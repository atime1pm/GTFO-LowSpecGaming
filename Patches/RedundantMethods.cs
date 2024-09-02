using Agents;
using ChainedPuzzles;
using CullingSystem;
using Enemies;
using FluffyUnderware.Curvy.Generator;
using FX_EffectSystem;
using HarmonyLib;
using Il2CppInterop.Runtime.Runtime;
using ShaderValueAnimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class RedundantMethods
    {
        public static bool draw;

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyUpdateManager), "GamestateUpdate")]
        [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.Update))]
        [HarmonyPatch(typeof(CL_Light), nameof(CL_Light.UpdateData))]
        [HarmonyPatch(typeof(CP_Bioscan_Graphics), nameof(CP_Bioscan_Graphics.Update))]
        public static IEnumerable<CodeInstruction> DisableTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>();
            code.Clear();
            return code.AsEnumerable();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Graphics), nameof(Graphics.DrawMeshInstancedIndirect),
            new Type[] {
                typeof(Mesh),typeof(int),typeof(Material),typeof(Bounds),typeof(ComputeBuffer),
                //mesh,submeshIndex,material,bounds,bufferWithArgs
                typeof(int), typeof(MaterialPropertyBlock),typeof(ShadowCastingMode),
                //argsOffset,properties,castShadows,
                typeof(bool),typeof(int),typeof(Camera),typeof(LightProbeUsage),typeof(LightProbeProxyVolume)})]
                //receiveShadows,layer,camera,lightProbeUsage,lightProbeProxyVolume,
        public static IEnumerable<CodeInstruction> DisableTranspiler2(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            if (draw) return code.AsEnumerable();

            code.Clear();
            return code.AsEnumerable();
        }
    }
}
