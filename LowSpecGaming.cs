using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using AssetShards;
using GameData;
using LowSpecGaming.Patches;
using BepInEx;
using Il2CppSystem.Data;

namespace LowSpecGaming
{
    internal class LowSpecGaming : MonoBehaviour
    {
        public static List<Texture2D> lightTextures;
        public static Texture2D text;
        public void Start() {
            EntryPoint.GetTheSettings();
            lightTextures = new();
            GTFO.API.LevelAPI.OnEnterLevel += HateTheGameFeel;
            GTFO.API.LevelAPI.OnEnterLevel += ClusterRenderingOff;

            //We apply settings 7 seconds after the game loads
            //to avoid the super glossy bug
            EntryPoint.LogIt("Applying settings in 12s");
            Invoke("ApplySettings",12f);
            Invoke("GetFlashLights", 10f);

        }
        public void GetFlashLights() {
            if (!EntryPoint.dumpTexture.Value) return;

            var lightsData = GameDataBlockBase<FlashlightSettingsDataBlock>.GetAllBlocks();
            foreach (var block in lightsData)
            {
                //lightTextures.Add(GTFO.API.AssetAPI.GetLoadedAsset<Texture2D>(block.cookie));
                lightTextures.Add(AssetShardManager.GetLoadedAsset(block.cookie).Cast<Texture2D>());
            }
            foreach (var l in lightTextures) 
            {
                try
                {
                    var tmp = RenderTexture.GetTemporary(l.width, l.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    var previous = RenderTexture.active;
                    Graphics.Blit(l, tmp);
                    RenderTexture.active = previous;
                    Texture2D newText = new(l.width, l.height);
                    previous = RenderTexture.active;
                    RenderTexture.active = tmp;

                    newText.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                    newText.Apply();
                    RenderTexture.ReleaseTemporary(tmp);
                    byte[] f = newText.EncodeToPNG();

                    var FolderPath = Paths.BepInExRootPath + "\\LowSpec\\";
                    if (!Directory.Exists(FolderPath))
                        Directory.CreateDirectory(FolderPath);


                    if (!Directory.Exists(FolderPath + "FlashLights\\"))
                        Directory.CreateDirectory(FolderPath + "FlashLights\\");

                    File.WriteAllBytes(FolderPath + "FlashLights\\" + l.name + ".png", f);
                }
                catch { EntryPoint.LogIt("Skipping Texture"); }
            }
        }
        public void ApplySettings() {
            int value = 10;
            EntryPoint.LogIt("Apply Settings");
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
            ClusteredRendering.Current.enabled = false; 
        }

    }
}
