using CullingSystem;
using Enemies;
using FluffyUnderware.Curvy.Examples;
using HarmonyLib;
using ShaderValueAnimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.ResolutionPatch
{
    [HarmonyPatch]
    internal class ResolutionPatch
    {
        public static bool dynamic = false;
        private static Vector3 lastAngle = new Vector3(0, 0, 0);
        private static Vector3 currentAngle = new Vector3(0, 0, 0);
        private static float scale = 0.5f;
        private static float offset = 0.25f;
        private static float mouse = 0f;

        static bool mouseMove = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LookCameraController), nameof(LookCameraController.MouseLookUpdate))]
        public static void Mouse(ref float axisHor, ref float axisVer)
        {
            mouse = Mathf.Abs(axisHor + axisVer);
            if (mouse == 0)
            {
                mouseMove = false;
                scale = 1f;
                offset = 0f;
            }
            else if (mouse < 2f)
            {
                mouseMove = true;
                scale = 0.8f;
                offset = 0.1f;
            }
            else if (mouse < 4f)
            {
                mouseMove = true;
                scale = 0.7f;
                offset = 0.15f;
            }
            else
            {
                mouseMove = true;
                scale = 0.6f;
                offset = 0.2f;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CL_Light), nameof(CL_Light.UpdateData))]
        public static bool LightCull(CL_Light __instance)
        {
            return false;

        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Awake))]
        public static void InitializeCulling(FPSCamera __instance)
        {
            __instance.gameObject.AddComponent<Culling>();
            __instance.gameObject.GetComponent<Culling>().enabled = false;

        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Update))]
        public static void ScaleDown(FPSCamera __instance)
        {
            
            LowSpecGaming.playerPos = __instance.transform.position;
            if (mouseMove)
            {
                if (LowSpecGaming.markerLayer != null)
                { LowSpecGaming.markerLayer.GetComponent<UI_Canvas>().CanvasScale = LowSpecGaming.canvasScale * scale; }
                Camera.current.rect = new Rect(offset, offset, scale, scale);
                Camera.current.farClipPlane = 100;//scaling down messes up the farclip so im demessing it
            }
            else
            {
                if (LowSpecGaming.markerLayer != null)
                { LowSpecGaming.markerLayer.GetComponent<UI_Canvas>().CanvasScale = LowSpecGaming.canvasScale; }
                Camera.current.farClipPlane = 64;//scaling down messes up the farclip so im demessing it
            }
            /*
            if (EntryPoint.dynamicResolution.Value)
            {
                
            }*/
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnPostRender))]
        public static void ScaleUp(FPSCamera __instance)
        {
            if (LowSpecGaming.markerLayer != null && EntryPoint.dynamicResolution.Value == false)
            { LowSpecGaming.markerLayer.GetComponent<UI_Canvas>().CanvasScale = LowSpecGaming.canvasScale; }
            Camera.current.rect = new Rect(0, 0, 1, 1);
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyUpdateManager), nameof(EnemyUpdateManager.Setup))]
        public static bool ScaleUDamnp(EnemyUpdateManager __instance)
        {
            EntryPoint.entry.Log.LogInfo("IT WAS ME WARIOOOO");
            if (!EntryPoint.enemyBehaviourCulling.Value)
            { return true; }
            ShaderValueAnimator.ManagerSetup();
            __instance.m_nodeUpdates = new EnemyUpdateManager.UpdateNodeGroup();
            __instance.m_nodeUpdates.Setup(1);
            __instance.m_locomotionUpdatesClose = new EnemyUpdateManager.UpdateLocomotionGroup();
            __instance.m_locomotionUpdatesClose.Setup(12);//20
            __instance.m_locomotionUpdatesNear = new EnemyUpdateManager.UpdateLocomotionGroup();
            __instance.m_locomotionUpdatesNear.Setup(3, __instance.m_locomotionUpdatesClose);//10
            __instance.m_locomotionUpdatesFar = new EnemyUpdateManager.UpdateLocomotionGroup();
            __instance.m_locomotionUpdatesFar.Setup(1, __instance.m_locomotionUpdatesNear);//5
            __instance.m_fixedLocomotionUpdatesClose = new EnemyUpdateManager.FixedUpdateLocomotionGroup();
            __instance.m_fixedLocomotionUpdatesClose.Setup(3);//5
            __instance.m_fixedLocomotionUpdatesNear = new EnemyUpdateManager.FixedUpdateLocomotionGroup();
            __instance.m_fixedLocomotionUpdatesNear.Setup(1, __instance.m_fixedLocomotionUpdatesClose);//2
            __instance.m_fixedLocomotionUpdatesFar = new EnemyUpdateManager.FixedUpdateLocomotionGroup();
            __instance.m_fixedLocomotionUpdatesFar.Setup(1, __instance.m_fixedLocomotionUpdatesNear);//1
            __instance.m_behaviourUpdatesClose = new EnemyUpdateManager.UpdateBehaviourGroup();
            __instance.m_behaviourUpdatesClose.Setup(1);//5
            __instance.m_behaviourUpdatesNear = new EnemyUpdateManager.UpdateBehaviourGroup();
            __instance.m_behaviourUpdatesNear.Setup(1, __instance.m_behaviourUpdatesClose);//2
            __instance.m_behaviourUpdatesFar = new EnemyUpdateManager.UpdateBehaviourGroup();
            __instance.m_behaviourUpdatesFar.Setup(1, __instance.m_behaviourUpdatesNear);//1
            __instance.m_detectionUpdatesClose = new EnemyUpdateManager.UpdateDetectionGroup();
            __instance.m_detectionUpdatesClose.Setup(10);//10
            __instance.m_detectionUpdatesNear = new EnemyUpdateManager.UpdateDetectionGroup();
            __instance.m_detectionUpdatesNear.Setup(3, __instance.m_detectionUpdatesNear);//4
            __instance.m_detectionUpdatesFar = new EnemyUpdateManager.UpdateDetectionGroup();
            __instance.m_detectionUpdatesFar.Setup(1, __instance.m_detectionUpdatesFar);//1
            __instance.m_networkUpdatesClose = new EnemyUpdateManager.UpdateNetworkGroup();
            __instance.m_networkUpdatesClose.Setup(12);//12
            __instance.m_networkUpdatesNear = new EnemyUpdateManager.UpdateNetworkGroup();
            __instance.m_networkUpdatesNear.Setup(3, __instance.m_networkUpdatesClose);//3
            __instance.m_networkUpdatesFar = new EnemyUpdateManager.UpdateNetworkGroup();
            __instance.m_networkUpdatesFar.Setup(1, __instance.m_networkUpdatesNear);//1
            return false;
        }

    }
}
