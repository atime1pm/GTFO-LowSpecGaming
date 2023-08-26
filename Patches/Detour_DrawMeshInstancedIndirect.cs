using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;
using Il2CppInterop.Runtime.Runtime;
using LowSpecGaming.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    [Harmony]
    [HarmonyPatch]
    internal static unsafe class Detour_DrawMeshInstancedIndirect
    {
        public static IntPtr DrawMeshCamera = IntPtr.Zero;
        public static bool draw = true;
        //IntPtr - Mesh mesh 
        //int - int submeshIndex
        //IntPtr - Material material
        //IntPtr - Bounds bounds
        //IntPtr - ComputeBuffer bufferWithArgs
        //int - int argsOffset
        //IntPtr - MaterialPropertyBlock properties
        //int - ShadowCastingMode castShadows
        //byte - bool receiveShadows
        //int - int layer
        //IntPtr - Camera camera
        //int - LightProbeUsage lightProbeUsage
        //IntPtr - LightProbeProxyVolume lightProbeProxyVolume
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
            if (!draw)
                return;
            if (camera == IntPtr.Zero)
            {
                camera = DrawMeshCamera;
            }

            _Original(
                mesh,
                submeshIndex,
                material,
                bounds,
                bufferWithArgs,
                argsOffset,
                properties,
                castShadows,
                receiveShadows,
                layer,
                camera,
                lightProbeUsage,
                lightProbeProxyVolume,
                methodInfo);
        }
    }
}
