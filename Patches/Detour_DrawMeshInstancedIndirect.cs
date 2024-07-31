using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace LowSpecGaming.Patches
{
    [Harmony]
    [HarmonyPatch]
    internal static unsafe class Detour_DrawMeshInstancedIndirect
    {
        public static IntPtr DrawMeshCamera = IntPtr.Zero;
        private delegate void DrawDelegate(
            IntPtr mesh,
            int submeshIndex,
            IntPtr material,
            IntPtr bounds,
            IntPtr bufferWithArgs,
            int argsOffset,
            IntPtr properties,
            int castShadows,
            byte receiveShadows,
            int layer,
            IntPtr camera,
            int lightProbeUsage,
            IntPtr lightProbeProxyVolume,
            Il2CppMethodInfo* methodInfo);

        private static DrawDelegate _Original;
        private static INativeDetour _Detour;

        public static void CreateDetour()
        {
            var description = new DetourDescription()
            {
                Type = typeof(Graphics),
                MethodName = nameof(Graphics.DrawMeshInstancedIndirect),
                ReturnType = typeof(void),
                ArgTypes = new Type[] { typeof(Camera) },
                IsGeneric = false
            };

            Detour.TryCreate(description, Detour_Draw, out _Original, out _Detour);
        }
        private static void Detour_Draw(IntPtr mesh,
            int submeshIndex,
            IntPtr material,
            IntPtr bounds,
            IntPtr bufferWithArgs,
            int argsOffset,
            IntPtr properties,
            int castShadows,
            byte receiveShadows,
            int layer,
            IntPtr camera,
            int lightProbeUsage,
            IntPtr lightProbeProxyVolume,
            Il2CppMethodInfo* methodInfo)
        {
            _Original(
                mesh,
                submeshIndex,
                material,
                bounds,
                bufferWithArgs,
                argsOffset,
                properties,
                0,
                0,
                layer,
                DrawMeshCamera,
                lightProbeUsage,
                lightProbeProxyVolume,
                methodInfo);
        }
    }
}
