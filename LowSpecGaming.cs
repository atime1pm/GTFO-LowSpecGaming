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

namespace Octomizer
{
    internal class LowSpecGaming : MonoBehaviour
    {
        private float updateInterval = 0.5f;      // Update interval for FPS calculation and GUI update
        private float accumulatedTime = 0f;    // Accumulated time since last FPS update
        private int frames = 0;                // Number of frames since last FPS update
        private float fps = 0f;                // Calculated FPS value
        public static float scale = 0.5f;   // The default scaling value
        public static float min = 0.5f;     // the minimum scaling value
        public static int maxFPS = 120;
        private float scaleFactor = 0.5f;
        public static float farClipScaled = 50 / min;
        public static Texture2D newnewSight;
        public static Texture2D oldSight;
        public static float canvasScale;

        private void Start() {
            GTFO.API.LevelAPI.OnEnterLevel += GetTheNav;
            if (EntryPoint.GameEnvironment.Value)
            { 
                GTFO.API.LevelAPI.OnEnterLevel += HateTheGameFeel;
            }
            ResolutionPatch.MaxTargetFrame();
            EntryPoint.GetTheSettings();
        }
        private void Update()
        {
            Task.Run(() => {
                frames++;
                accumulatedTime += Time.unscaledDeltaTime;
                if (accumulatedTime >= updateInterval)
                {
                    fps = frames / accumulatedTime;
                    min = Mathf.Clamp(min, 0.1f, 1f);//ugly code... please dont look
                    scaleFactor = Mathf.Clamp((Mathf.Floor((fps / maxFPS) * 10) / 10), min, 1f);
                    if (scale < scaleFactor)
                    { scale += 0.05f; }
                    else if (scale > scaleFactor)
                    { scale -= 0.05f; }
                    frames = 0;
                    accumulatedTime = 0f;
                }
            });
        }
        private void GetTheNav() {
            DrawPatch.markerLayer = GameObject.Find("NavMarkerLayer");
            maxFPS = CellSettingsManager.SettingsData.Video.TargetFramerate.Value;

        }
        private void HateTheGameFeel() {
            PreLitVolume.Current.gameObject.GetComponent<AmbientParticles>().enabled = false;
            PreLitVolume.Current.m_fogDistance = 20;
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
