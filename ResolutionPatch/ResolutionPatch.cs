using CullingSystem;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LowSpecGaming.Patches;
using UnityEngine.UIElements.UIR;
using LowSpecGaming.Misc;

namespace LowSpecGaming.ResolutionPatch
{
    [HarmonyPatch]
    internal class ResolutionPatch
    {
        public static bool dynamic = false;
        
        public static UI_Canvas markerLayer = null;
        public static float canvasScale;

        private static float scale = 0.8f;
        private static float offset = 0.1f;
        private static float mouse = 0f;
        
        static bool mouseMove = false;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LookCameraController), nameof(LookCameraController.MouseLookUpdate))]
        public static void Mouse(ref float axisHor, ref float axisVer)
        {
            mouse = Mathf.Abs(axisHor + axisVer);
            if (mouse < 1f)
            {
                scale = 1f;
                offset = 0f;
            }
            else if (mouse < 2f)
            {
                scale = 0.8f;
                offset = 0.1f;
            }
            else if (mouse > 2f)
            {
                scale = 0.7f;
                offset = 0.15f;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Awake))]
        public static void InitializeCulling(FPSCamera __instance)
        {
            //__instance.gameObject.AddComponent<Culling>();
            Detour_DrawMeshInstancedIndirect.DrawMeshCamera = __instance.m_camera.Pointer;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Update))]
        public static void ScaleDown(FPSCamera __instance)
        {
            //Culling.playerPos = __instance.transform.position;
            if (!dynamic)
                return;
            markerLayer.CanvasScale = canvasScale * scale;
            Camera.current.rect = new Rect(offset, offset, scale, scale);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnPostRender))]
        public static void ScaleUp(FPSCamera __instance)
        {
            markerLayer.CanvasScale = canvasScale;
            Camera.current.rect = new Rect(0, 0, 1, 1);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AkAudioListener), nameof(AkAudioListener.OnEnable))]
        public static void SoundFix(AkAudioListener __instance)
        {
        }
    }
}
