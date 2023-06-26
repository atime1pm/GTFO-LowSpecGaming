using FluffyUnderware.DevTools.Extensions;
using HarmonyLib;
using IRF;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Enemies;
using Dissonance;
using GTFO;
using UnityEngine.Rendering;
using UnityEngine.PostProcessing;
using System.Runtime.CompilerServices;
using Il2CppSystem.Collections;
using System.Text.RegularExpressions;
using Decals;
using ShaderValueAnimation;
using System.Reflection;
using GameData;
using Il2CppInterop.Runtime.Injection;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.RuntimeDetour;
using static UnLogickFactory.FbxTextureExportScheme;

namespace Octomizer
{
    [HarmonyPatch]
    internal class DrawPatch
    {
        public static GameObject markerLayer = null;
        static IntPtr nullCamera = IntPtr.Zero;
        public static bool dynamic = false;
        static bool drawing = true;
        static int textureSize = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InstancedRenderFeatureRenderer), nameof(InstancedRenderFeatureRenderer.Draw))]
        public static bool DrawIRF(ref IntPtr camera)
        {//I hate R7D1 fr fr
            return EntryPoint.TreeDrawing.Value;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InstancedRenderFeatureRenderer), nameof(InstancedRenderFeatureRenderer.Update))]
        public static bool UpdateIRF()
        {
            return EntryPoint.TreeDrawing.Value;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Decals.Decal), nameof(Decals.Decal.OnEnable))]
        public static void ShutUpSpitter(Decal __instance)
        {
            if (!(EntryPoint.HateSpitter.Value))
            { return; }
            if (__instance.gameObject.name.ToLower().Contains("spitter"))
            {
                Material mat = __instance.GetComponentInParent<InfectionSpitter>().m_renderer.material;
                mat.GetTexture(1).mipMapBias = 10;
                mat.GetTexture("_MetallicGlossMap").mipMapBias = 10;
                mat.GetTexture("_SSSMap").mipMapBias = 10;
                mat.GetTexture("_MainTex_B").mipMapBias = 10;
                Task.Run(__instance.Destroy);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CellSettingsApply), nameof(CellSettingsApply.ApplyTextureSize))]
        public static bool PotatoTexture(ref int value)
        {
            EntryPoint.GetTheSettings();
            value = EntryPoint.TextureSize.Value;
            if (QualitySettings.masterTextureLimit != value)
            {
                QualitySettings.masterTextureLimit = value;
            }
            return false;
        }
    }
}

