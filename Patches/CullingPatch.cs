using BepInEx.Unity.IL2CPP.Hook;
using CullingSystem;
using Enemies;
using HarmonyLib;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class CullingPatch
    {
        private delegate void ShowDelegate(IntPtr __instance);
        private delegate void HideDelegate(IntPtr __instance);

        private static ShowDelegate _OriginalShow;
        private static INativeDetour _DetourShow;
        private static HideDelegate _OriginalHide;
        private static INativeDetour _DetourHide;

        public static void CreateDetour()
        {
            Detour.TryCreate(new DetourDescription()
            {
                Type = typeof(C_CullingCluster),
                MethodName = nameof(C_CullingCluster.Show),
                ReturnType = typeof(void),
                ArgTypes = new Type[] { },
                IsGeneric = false
            }, Detour_Show, out _OriginalShow, out _DetourShow);
            Detour.TryCreate(new DetourDescription()
            {
                Type = typeof(C_CullingCluster),
                MethodName = nameof(C_CullingCluster.Hide),
                ReturnType = typeof(void),
                ArgTypes = new Type[] { },
                IsGeneric = false
            }, Detour_Hide, out _OriginalHide, out _DetourHide);
        }
        public static void Detour_Show(IntPtr __instance)
        {
            C_CullingCluster c = new(__instance);
            if (c.IsShown) return;

            c.IsShown = true;
            
            foreach (Renderer r in c.Renderers)
                r.enabled = true;
            
            C_CullingManager.Register(c);
            return;
        }
        public static void Detour_Hide(IntPtr __instance)
        {
            C_CullingCluster c = new(__instance);

            if (!c.IsShown) return;

            c.IsShown = false;
            
            foreach (Renderer r in c.Renderers)
                r.enabled = false;
            
            return;
        }
    }
}
