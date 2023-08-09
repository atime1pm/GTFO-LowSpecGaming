using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;
using FluffyUnderware.DevTools.Extensions;
using Enemies;
using AssetShards;
using GameData;
using LevelGeneration;
using LowSpecGaming.ResolutionPatch;
using System.Diagnostics.CodeAnalysis;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace LowSpecGaming
{
    internal class LowSpecGaming : MonoBehaviour
    {
        public static Vector3 playerPos;
        public static GameObject markerLayer = null;
        public static
        private void Start() {
            GTFO.API.LevelAPI.OnEnterLevel += GetTheNav;

            if (EntryPoint.GameEnvironment.Value)
            { 
                GTFO.API.LevelAPI.OnEnterLevel += HateTheGameFeel;
            }
            StartUpSettings.MaxTargetFrame();
            EntryPoint.GetTheSettings();
            Invoke("ApplySettings", 7f);

        }
        public void ApplySettings() {
            int value = EntryPoint.TextureSize.Value;
            StartUpSettings.gameLoaded = true;
            StartUpSettings.PotatoTexture(ref value);
        }
        private void Update()
        {
        }
        private void GetTheNav() {
            markerLayer = GameObject.Find("NavMarkerLayer");
        }
        private void HateTheGameFeel() {
            PreLitVolume.Current.gameObject.GetComponent<AmbientParticles>().enabled = false;
            PreLitVolume.Current.m_fogDistance = 45;
            //PreLitVolume.Current.FogAACount = 0;
            PreLitVolume.Current.FogPostBlur= 1;
            PreLitVolume.Current.FogShadowSamples = 0;
            PreLitVolume.Current.IndirectBlurSamples = 0;
            PreLitVolume.Current.IndirectDownsampling= 0;
            QualitySettings.shadowDistance = 15;
            QualitySettings.softParticles = false;
        }
    }
}
