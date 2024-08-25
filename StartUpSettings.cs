using HarmonyLib;
using System;
using UnityEngine;
using Dissonance;
using System.Runtime.CompilerServices;
using Il2CppSystem.Collections;
using LowSpecGaming.Patches;
using LowSpecGaming.Misc;
using GameData;
using Gear;
using AssetShards;

namespace LowSpecGaming
{
    [HarmonyPatch]
    internal class StartUpSettings
    {
        //We make the game loads at full texture first
        //This prevent the SUPER GLOSS error
        //
        [HarmonyPostfix][HarmonyPatch(typeof(StartMainGame), nameof(StartMainGame.Start))]
        public static void MakeItWork(StartMainGame __instance)
        {
            __instance.gameObject.AddComponent<LowSpecGaming>();
            QualitySettings.masterTextureLimit = 0;
        }


        //We apply all the settings here
        //
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CellSettingsApply), nameof(CellSettingsApply.ApplyTextureSize))]
        public static bool PotatoTexture(ref int value)
        {
            EntryPoint.GetTheSettings();

            ResolutionPatch.dynamic = EntryPoint.dynamicResolution.Value == DynamicResolution.Dynamic;

            //Find The player layer of the GUI, this is how marker gets scale right with the camera
            //
            ResolutionPatch.markerLayer = GameObject.Find("GUI").transform.GetChild(0).GetChild(0).GetComponent<UI_Canvas>();
            //Get the scaling of how the canvas is supposed to scale, some how this value is scale according to 1080p
            //
            ResolutionPatch.canvasScale = CellSettingsManager.SettingsData.Video.Resolution.Value.y / 1080f;


            SpitterPatch.hate = EntryPoint.hateSpitter.Value == HateSpitter.HATE;
            BioScanPatch.update = EntryPoint.BioScanUpdate.Value == BioScanBlink.Blink;
            IRFPatch.draw = EntryPoint.treeDrawing.Value == TreeDrawing.Draw;

            //RedundantMethods.experimentalOn = EntryPoint.redundantComponents.Value == Experimental.TurnOn;

            //This prevents the texture from reloading whenever you change settings
            //
            if (EntryPoint.dumpTexture.Value)
            {
                QualitySettings.masterTextureLimit = 0;
            }
            else
            {
                value = (int)EntryPoint.textureSize.Value;
                if (QualitySettings.masterTextureLimit != value)
                    QualitySettings.masterTextureLimit = value;
            }
            
            return false;
        }
    }
}

