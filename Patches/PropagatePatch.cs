using CullingSystem;
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
        public static bool stopper = true;
        public static bool stopper2 = true;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_Camera), nameof(C_Camera.RunVisibility))]
        public static bool RunVisibilityPatch(C_Camera __instance)
        {
            if (C_Camera.RenderWithElevatorMask || __instance.m_cullAgent.CurrentNode == null)
                return false;
            C_Camera.Position = __instance.transform.position;
            C_Camera.Forward = __instance.transform.forward;
            C_Camera.Right = __instance.transform.right;
            C_Camera.Up = __instance.transform.up;
            //C_CullingManager.SpotLights_WantsToShow.Clear();
            //C_CullingManager.PointLights_WantsToShow.Clear();
            //C_LightPropagationJob.ResetDynamics();
            //__instance.PropagateFrustra();
            //C_CullingClusterPatch.Pattern_Cull(C_Camera.Position);
            //C_CullingManager.SortAndUpdateLights();
            //if (stopper6) C_CullingManager.C_CameraUpdate();
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_Node), nameof(C_Node.CullContents))]
        public static bool PatchCou(C_Node __instance, ref C_FrustumData fd)
        {
            __instance.CullKey = C_Keys.CurrentCullKey;
            //__instance.Show();
            __instance.ProcessLights(fd, __instance.m_cLights);
            //__instance.ProcessBuckets(fd);
            __instance.ProcessMovingCullers(fd, __instance.m_movingCullers);
            return false;
        }

        [HarmonyPostfix][HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Awake))]
        public static void AddPatchToPlayer(FPSCamera __instance)
        {
            __instance.gameObject.AddComponent<C_CullingClusterPatch>();
        }
    }
}
