using HarmonyLib;
using System;
using UnityEngine;
using Dissonance;
using System.Runtime.CompilerServices;
using Il2CppSystem.Collections;
using LowSpecGaming.Patches;
using LowSpecGaming.ResolutionPatch;
using LowSpecGaming.Misc;
using LowSpecGaming.Structs;

namespace LowSpecGaming
{
    [HarmonyPatch]
    internal class StartUpSettings
    {
        public static bool gameLoaded = false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartMainGame), nameof(StartMainGame.Start))]
        public static void MakeItWork(StartMainGame __instance)
        {
            __instance.gameObject.AddComponent<LowSpecGaming>();//at least it works
            QualitySettings.masterTextureLimit = 0;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CellSettingsApply), nameof(CellSettingsApply.ApplyTextureSize))]
        public static bool PotatoTexture(ref int value)
        {
            EntryPoint.GetTheSettings();
            ResolutionPatch.ResolutionPatch.markerLayer = GameObject.Find("GUI").transform.GetChild(0).GetChild(0).GetComponent<UI_Canvas>();
            Detour_DrawMeshInstancedIndirect.draw = EntryPoint.treeDrawing.Value == TreeDrawing.Draw ? true:false ;
            ResolutionPatch.ResolutionPatch.dynamic = EntryPoint.dynamicResolution.Value == DynamicResolution.Dynamic ? true:false ;
            ResolutionPatch.ResolutionPatch.canvasScale = CellSettingsManager.SettingsData.Video.Resolution.Value.y / 1080f;
            SpitterPatch.hate = EntryPoint.hateSpitter.Value == HateSpitter.HATE ? true :false;
            BioScanPatch.update = EntryPoint.BioScanUpdate.Value == BioScanBlink.Blink ? true: false;

            value = (int)EntryPoint.textureSize.Value;
            if (QualitySettings.masterTextureLimit != value)
            {
                QualitySettings.masterTextureLimit = value;
            }
            return false;
        }
    }
}

