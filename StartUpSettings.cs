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
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using AssetShards;

namespace LowSpecGaming
{
    [HarmonyPatch]
    internal class StartUpSettings
    {
        public static bool dynamic = false;
        public static float initialScale = 0f;
        public static float testScale = 0.5f;
        public static bool gameLoaded = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartMainGame), nameof(StartMainGame.Awake))]
        public static void MakeItWork()
        {
            GameObject octomizer = new GameObject();//Essential and IDK WHAT THE FUCKKKKKKK
            octomizer.AddComponent<LowSpecGaming>();//at least it works
            QualitySettings.masterTextureLimit = 0;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CellSettingsApply), nameof(CellSettingsApply.ApplyTargetFramerate))]
        public static void MaxTargetFrame()
        {
            dynamic = EntryPoint.dynamicResolution.Value;
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

