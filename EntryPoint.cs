using BepInEx.Unity.IL2CPP;
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
using LowSpecGaming.ResolutionPatch;
using LowSpecGaming.Misc;
using PluginInfo = LowSpecGaming.Misc.PluginInfo;
using LowSpecGaming.Patches;
using LowSpecGaming.Structs;
using static UnLogickFactory.FbxTextureExportScheme;

namespace LowSpecGaming
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class EntryPoint : BasePlugin
    {
        public static ConfigEntry<DynamicResolution> dynamicResolution;
        public static ConfigEntry<TreeDrawing> treeDrawing;
        public static ConfigEntry<GameEnvironment> gameEnvironment;
        public static ConfigEntry<BioScanBlink> BioScanUpdate;
        public static ConfigEntry<HateSpitter> hateSpitter;
        public static ConfigEntry<EnemyBehaviourCulling> enemyBehaviourCulling;
        public static ConfigEntry<string> currentFolderPath;
        public static ConfigEntry<TextureSize> textureSize;
        public static ConfigFile configFile;
        public static Dictionary<string, string[]> sightPaths = new Dictionary<string, string[]>();

        public override void Load()
        {
            entry = this;
            m_Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            this.Log.LogInfo("Mushroom Low Spec Gaming is IN~!!");
            ClassInjector.RegisterTypeInIl2Cpp<LowSpecGaming>();
            ClassInjector.RegisterTypeInIl2Cpp<Culling>();
            GetTheSettings();
            //Detour_DrawMeshInstancedIndirect.CreateDetour();
            GetSightFolders();
            WeaponPatch.data = File.ReadAllBytes(sightPaths["GunFlashLight"][0]);
            m_Harmony.PatchAll();
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
            dynamicResolution = configFile.Bind<DynamicResolution>("Setup", nameof(dynamicResolution), DynamicResolution.Stable, "Scales down your resolution whenever you move your camera, will improve this in the future");
            treeDrawing = configFile.Bind<TreeDrawing>("Setup", nameof(treeDrawing), TreeDrawing.Draw, "whether or not to draw IRF like trees and tentacles");
            gameEnvironment = configFile.Bind<GameEnvironment>("Setup", nameof(gameEnvironment), GameEnvironment.Full, "Reduce fog distance, shadow distance, no more dust particles (requires restart to take effect)");
            BioScanUpdate = configFile.Bind<BioScanBlink>("Setup", nameof(BioScanUpdate), BioScanBlink.DontBlink, "Whether or not Bio Scans would blink, courtesy to McCad, this is his online code");
            hateSpitter = configFile.Bind<HateSpitter>("Setup", nameof(hateSpitter), HateSpitter.HATE, "If you hate spitters, make them low quality");

            enemyBehaviourCulling = configFile.Bind<EnemyBehaviourCulling>("Setup", nameof(enemyBehaviourCulling), EnemyBehaviourCulling.Full, "Reduce Enemy Update in order to save performance, will improve this feature in the future for better performance");
            textureSize = configFile.Bind<TextureSize>("Setup", nameof(textureSize), TextureSize.Full, "Texture size, for the potata army");
            currentFolderPath = configFile.Bind<string>("Setup", nameof(currentFolderPath),"" , "Manual Path to the current folder that has the plugin if it fails to load normally, This will be removed soon");

        }
        public static void GetSightFolders()
        {
            string currentFolder = null;
            string thisFolder = null;
            string[] sightFolder = null;
            entry.Log.LogInfo("Trying to find sights");

            //UGlies code ive ever written in my life
            if (currentFolderPath.Value.Equals("") || currentFolderPath.Value == null)
            {
                entry.Log.LogInfo("Auto Loaded");
                foreach (string folder in Directory.GetDirectories(Paths.PluginPath))
                { if (folder.Contains(PluginInfo.PLUGIN_NAME)) { currentFolder = folder; } }
            }
            else
            {
                entry.Log.LogInfo("Manually Loaded");
                currentFolder = currentFolderPath.Value;
            }

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

        public static void LogIt(object data)
        {
            entry.Log.LogInfo(data);
        }
        public static void DecompressFolder(string compressedFilePath, string decompressedFolderPath)
        {
            // Extract the contents of the ZIP archive to a folder
            ZipFile.ExtractToDirectory(compressedFilePath, decompressedFolderPath);

            entry.Log.LogInfo("Folder decompressed successfully.");
        }
    }

}
