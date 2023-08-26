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
using CullingSystem;
using FluffyUnderware.Curvy.Generator;

namespace LowSpecGaming.Misc
{
    [HarmonyPatch]
    internal static class SpitterPatch
    {
        public static bool hate = true;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Decal), nameof(Decal.OnEnable))]
        public static void ShutUpSpitter(Decal __instance)
        {
            if (!hate)
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
        [HarmonyPatch(typeof(InfectionSpitter), nameof(InfectionSpitter.StopPurring))]
        public static void StopPurring(InfectionSpitter __instance)
        {
            __instance.m_purrLoopPlaying = true;
        }
    }
}

