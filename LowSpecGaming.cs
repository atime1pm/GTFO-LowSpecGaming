using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;
using FluffyUnderware.DevTools.Extensions;
using AssetShards;
using GameData;
using System.Diagnostics.CodeAnalysis;
using LowSpecGaming.Patches;

namespace LowSpecGaming
{
    internal class LowSpecGaming : MonoBehaviour
    {
        public void Awake() {
            EntryPoint.GetTheSettings();

            GTFO.API.LevelAPI.OnEnterLevel += HateTheGameFeel;
            GTFO.API.LevelAPI.OnEnterLevel += ClusterRenderingOff;
            GTFO.API.LevelAPI.OnLevelCleanup += CheckSound;

            //We apply settings 7 seconds after the game loads
            //to avoid the super glossy bug
            Invoke("ApplySettings", 7f);
        }

        //Make sure to turn the sound back on
        //
        public static void CheckSound() => Camera.main.GetComponent<AkAudioListener>().enabled = true;
        public static void ApplySettings() {
            int value = (int)EntryPoint.textureSize.Value;
            EntryPoint.LogIt("Apply Settings");
            ResolutionPatch.canvasScale = CellSettingsManager.SettingsData.Video.Resolution.Value.y / 1080f;
            StartUpSettings.PotatoTexture(ref value);
        }

        private static void HateTheGameFeel() {
            if (EntryPoint.gameEnvironment.Value == GameEnvironment.Full) return;

            PreLitVolume.Current.gameObject.GetComponent<AmbientParticles>().enabled = false;
            PreLitVolume.Current.m_fogDistance = 45;
            PreLitVolume.Current.FogPostBlur= 1;
            PreLitVolume.Current.FogShadowSamples = 0;
            PreLitVolume.Current.IndirectBlurSamples = 0;
            PreLitVolume.Current.IndirectDownsampling= 0;
            QualitySettings.shadowDistance = 15;
            QualitySettings.softParticles = false;
        }

        //Not sure to use this or not
        //But this could give us around 5fps+
        //
        public static void ClusterRenderingOff() 
        {
            if (EntryPoint.redundantComponents.Value == Experimental.TurnOn) return;
            ClusteredRendering.Current.enabled = false; 
        }

    }
}
