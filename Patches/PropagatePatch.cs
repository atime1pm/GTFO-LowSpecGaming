using ChainedPuzzles;
using CullingSystem;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class PropagatePatch
    {
        [HarmonyPostfix][HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Awake))]
        public static void AddPatchToPlayer(FPSCamera __instance)
        {
            __instance.gameObject.AddComponent<C_CullingClusterPatch>();
        }
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(C_Node), nameof(C_Node.ProcessBuckets))]
        [HarmonyPatch(typeof(C_CullingCluster), nameof(C_CullingCluster.Show))]
        //[HarmonyPatch(typeof(C_Camera), nameof(C_Camera.RunVisibility))]
        public static IEnumerable<CodeInstruction> DisableTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            code.Clear();
            return code.AsEnumerable();
        }
    }
}
