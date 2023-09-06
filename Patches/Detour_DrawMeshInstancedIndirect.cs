using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Linq;
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
            if (!draw) return;

            camera = DrawMeshCamera;
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

    public static unsafe class Detour
    {
        public delegate void StaticVoidDelegate(Il2CppMethodInfo* methodInfo);
        public delegate void InstanceVoidDelegate(IntPtr instance, Il2CppMethodInfo* methodInfo);

        public static bool TryCreate<T>(DetourDescription description, T to, out T originalCall, out INativeDetour detourInstance) where T : Delegate
        {
            try
            {
                detourInstance = INativeDetour.CreateAndApply(description.GetMethodPointer(), to, out originalCall);
                return detourInstance != null;
            }
            catch { }

            originalCall = null;
            detourInstance = null;
            return false;
        }
    }

    public struct DetourDescription
    {
        public Type Type;
        public Type ReturnType;
        public Type[] ArgTypes;
        public string MethodName;
        public bool IsGeneric;

        public unsafe nint GetMethodPointer()
        {
            if (Type == null)
            {
                throw new MissingFieldException($"Field {nameof(Type)} is not set!");
            }

            if (ReturnType == null)
            {
                throw new MissingFieldException($"Field {nameof(ReturnType)} is not set! If you mean 'void' do typeof(void)");
            }

            if (string.IsNullOrEmpty(MethodName))
            {
                throw new MissingFieldException($"Field {nameof(MethodName)} is not set or valid!");
            }

            var type = Il2CppType.From(Type, throwOnFailure: true);
            var typePtr = Il2CppClassPointerStore.GetNativeClassPointer(Type);

            var returnType = Il2CppType.From(ReturnType, throwOnFailure: true);
            Il2CppSystem.Type[] il2cppArgTypes;
            if (ArgTypes == null || ArgTypes.Length <= 0)
            {
                il2cppArgTypes = Array.Empty<Il2CppSystem.Type>();
            }
            else
            {
                var length = ArgTypes.Length;
                il2cppArgTypes = new Il2CppSystem.Type[length];
                for (int i = 0; i < length; i++)
                {
                    var argType = ArgTypes[i];
                    il2cppArgTypes[i] = Il2CppType.From(argType, throwOnFailure: true);
                }
            }

            var argStrArray = il2cppArgTypes.Select(t => t.FullName).ToArray();
            var methodPtr = (void**)IL2CPP.GetIl2CppMethod(typePtr, IsGeneric, MethodName, returnType.FullName, argStrArray).ToPointer();
            if (methodPtr == null)
            {
                return (nint)methodPtr;
            }

            return (nint)(*methodPtr);
        }
    }
}
