﻿using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using BepInEx;
using Il2CppInterop.Runtime.Injection;
using BepInEx.Logging;
using BepInEx.Configuration;
using LevelGeneration;
using UnityEngine;
using System.IO.Compression;
using Dissonance;
using AssetShards;

namespace Octomizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class EntryPoint : BasePlugin
    {
        public static ConfigEntry<bool> dynamicResolution;
        public static ConfigEntry<float> dynamicResolutionMinimum;
        public static ConfigEntry<bool> TreeDrawing;
        public static ConfigEntry<bool> GameEnvironment;
        public static ConfigEntry<bool> BioScanUpdate;
        public static ConfigEntry<bool> HateSpitter;

        public static ConfigEntry<int> TextureSize;
        public static ConfigFile configFile;
        public static Dictionary<string, string[]> sightPaths = new Dictionary<string, string[]>();

        public override void Load()
        {
            entry = this;
            m_Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            this.Log.LogInfo("Mushroom Low Spec Gaming is IN~!!");
            ClassInjector.RegisterTypeInIl2Cpp<LowSpecGaming>();

            GetSightFolders();

            m_Harmony.PatchAll();
            GetTheSettings();

        }
        public static EntryPoint entry;
        private Harmony m_Harmony;
        public override bool Unload()
        {
            return base.Unload();
        }
        public static void GetTheSettings()
        {
            configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "mushroom.lowspecgaming.cfg"), true);
            dynamicResolution = configFile.Bind<bool>("Setup", nameof(dynamicResolution), false, "Whether or not to use dynamic resolution, will improve this in the future");
            dynamicResolutionMinimum = configFile.Bind<float>("Setup", nameof(dynamicResolutionMinimum), 0.7f, "The minimum of dynamic resolution");
            TreeDrawing = configFile.Bind<bool>("Setup", nameof(TreeDrawing), false, "whether or not to draw IRF like trees and tentacles");
            GameEnvironment = configFile.Bind<bool>("Setup", nameof(GameEnvironment), true, "Reduce fog distance, shadow distance, no more dust particles (requires restart to take effect)");
            BioScanUpdate = configFile.Bind<bool>("Setup", nameof(BioScanUpdate), false, "Whether or not Bio Scans would blink, courtesy to McCad, this is his online code");
            HateSpitter = configFile.Bind<bool>("Setup", nameof(HateSpitter), true, "If you hate spitters, make them low quality");

            TextureSize = configFile.Bind<int>("Setup", nameof(TextureSize), 0, "Texture size,the higher you go the lower the resolution, max is 10");
            LowSpecGaming.min = dynamicResolutionMinimum.Value;
            DrawPatch.dynamic = dynamicResolution.Value;
        }
        public static void GetSightFolders()
        {
            string currentFolder = null;
            string thisFolder = null;
            string[] sightFolder = null;
            entry.Log.LogInfo("Trying to find sights");

            //UGlies code ive ever written in my life
            foreach (string folder in Directory.GetDirectories(Paths.PluginPath))
            { if (folder.Contains(PluginInfo.PLUGIN_NAME)) { currentFolder = folder; } }

            if (!(Directory.Exists(Path.Combine(currentFolder, "Sight"))))
            {
                try { 
                    DecompressFolder(Path.Combine(currentFolder, "Sight.gtfo"), currentFolder); }
                catch (Exception e) { }
            }//More ugli codes

            thisFolder = Path.Combine(currentFolder, "Sight");
            entry.Log.LogInfo("Found Sights");

            sightFolder = Directory.GetDirectories(thisFolder);

            if (sightFolder != null)
            {
                foreach (string gearSight in sightFolder)
                {
                    if (gearSight.ToLower().Contains("pr 11"))
                    {
                        string newSightFolder = gearSight.Substring(0, gearSight.Length - 21);
                            newSightFolder += "GearItem_Köning PR 11";
                            if (gearSight != newSightFolder)
                            {
                                Directory.Move(gearSight, newSightFolder);
                                entry.Log.LogInfo("Sniper fixed, it was the compression's fault");
                            }
                            string currentGear = newSightFolder.Split("\\")[newSightFolder.Split("\\").Count<String>() - 1];
                            sightPaths[currentGear] = Directory.GetFiles(newSightFolder);
                        }
                        else
                        {
                            string currentGear = gearSight.Split("\\")[gearSight.Split("\\").Count<String>() - 1];
                            sightPaths[currentGear] = Directory.GetFiles(gearSight);
                        }

                    }
                }
        }


        public static void DecompressFolder(string compressedFilePath, string decompressedFolderPath)
        {
            // Extract the contents of the ZIP archive to a folder
            ZipFile.ExtractToDirectory(compressedFilePath, decompressedFolderPath);

            entry.Log.LogInfo("Folder decompressed successfully.");
        }
    }
}
