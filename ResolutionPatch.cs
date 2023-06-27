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

namespace Octomizer
{
    [HarmonyPatch]
    internal class ResolutionPatch
    {
        public static GameObject markerLayer = null;
        public static bool dynamic = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartMainGame), nameof(StartMainGame.Awake))]
        public static void MakeItWork()
        {
            GameObject octomizer = new GameObject();//Essential and IDK WHAT THE FUCKKKKKKK
            octomizer.AddComponent<LowSpecGaming>();//at least it works
            octomizer.name = "Octomizer";
            if (QualitySettings.masterTextureLimit != EntryPoint.TextureSize.Value)
            {
                QualitySettings.masterTextureLimit = EntryPoint.TextureSize.Value;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CellSettingsApply), nameof(CellSettingsApply.ApplyTargetFramerate))]
        public static void MaxTargetFrame()
        {
            LowSpecGaming.maxFPS = CellSettingsManager.SettingsData.Video.TargetFramerate.Value;
            LowSpecGaming.canvasScale = CellSettingsManager.SettingsData.Video.Resolution.Value.y / 1080;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.Update))]
        public static void SCALEDOWN(FPSCamera __instance)
        {
            __instance.m_camera.cullingMatrix = __instance.m_camera.projectionMatrix * __instance.m_camera.worldToCameraMatrix;
            if (EntryPoint.dynamicResolution.Value)
            {
                if (markerLayer != null)
                { markerLayer.GetComponent<UI_Canvas>().CanvasScale = LowSpecGaming.canvasScale * LowSpecGaming.scale; }
                __instance.m_camera.rect = new Rect((1 - LowSpecGaming.scale) / 2, (1 - LowSpecGaming.scale) / 2, LowSpecGaming.scale, LowSpecGaming.scale);//UGLY CODE
                __instance.m_camera.farClipPlane = 50 / LowSpecGaming.min;//scaling down messes up the farclip so im demessing it
                if (Camera.allCamerasCount > 1) { __instance.m_camera.farClipPlane = 1f; }//At least dont render too much when im tabout 
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FPSCamera), nameof(FPSCamera.OnPostRender))]

        public static void SCALEUP(FPSCamera __instance)
        {
            if (markerLayer != null && EntryPoint.dynamicResolution.Value == false)
            { markerLayer.GetComponent<UI_Canvas>().CanvasScale = LowSpecGaming.canvasScale; }
            __instance.m_camera.rect = new Rect(0, 0, 1, 1);//UPSCALE, WHERE"S MY FSR????I GOTTA CODE IT MYSELF????
        }
    }
}

