using CullingSystem;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements.UIR;
using LowSpecGaming.Misc;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class ResolutionPatch
    {
        public static bool dynamic = false;

        public static UI_Canvas markerLayer = null; //Playerlayer of GUI
        public static float canvasScale;// the initial scale of the playerlayer

        private static float scale = 0.8f; // camera scale
        private static float offset = 0.1f;// this is (1 - camera scale) / 2
        private static Rect screenRec = new (0f,0f,1f,1f);// this is (1 - camera scale) / 2

        //Mouse Movement Detection
        //I dont want to do any calculation here to save 0.001% performance
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LookCameraController), nameof(LookCameraController.MouseLookUpdate))]
        public static void Mouse(ref float axisHor, ref float axisVer)
        {
            float mouse = Mathf.Abs(axisHor) + Mathf.Abs(axisVer);

            if (mouse < 0.5f)
            {
                scale = 1f;
                offset = 0f;
            }
            else if (mouse < 2f)
            {
                scale = 0.8f;
                offset = 0.1f;
            }
            else
            {
                scale = 0.6f;
                offset = 0.2f;
            }
        }
        //Why no Detour for this?
        //Using Detour crashes whenever I use UnityExplorer with this
        //
        [HarmonyPostfix][HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Update))]
        public static void ScaleDown(FPSCamera __instance)
        {
            if (!dynamic) return;

            markerLayer.CanvasScale = canvasScale * scale;
            __instance.m_camera.rect = new Rect(offset, offset, scale, scale);
            //__instance.m_camera.rect.Set(offset, offset, scale, scale);
        }
        [HarmonyPostfix][HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnPostRender))]
        public static void ScaleUp(FPSCamera __instance)
        {
            markerLayer.CanvasScale = canvasScale;
            __instance.m_camera.rect = new Rect(0, 0, 1, 1);
        }

        //This is for the IRF
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Awake))]
        public static void DrawDetour(FPSCamera __instance) => Detour_DrawMeshInstancedIndirect.DrawMeshCamera = __instance.m_camera.Pointer;
    }
}
