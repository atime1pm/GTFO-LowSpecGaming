using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using BepInEx;
using Il2CppInterop.Runtime.Injection;
using BepInEx.Configuration;
using System.IO.Compression;
using LowSpecGaming.Patches;
using GameData;
using UnityEngine;
using Enemies;

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
        public static ConfigEntry<bool> dumpTexture;
        public static ConfigEntry<TextureSize> textureSize;
        public static ConfigEntry<OneFlashLight> oneFlashLight;
        public static ConfigEntry<Experimental> redundantComponents;
        public static ConfigFile configFile;

        public override void Load()
        {
            e = this;
            var m_Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            Log.LogInfo("Mushroom Low Spec Gaming is IN~!!");
            
            GetTheSettings();
            
            ClassInjector.RegisterTypeInIl2Cpp<LowSpecGaming>();
            SightPatch.sightPaths = new();
            SightPatch.GetSightFolders();
            LogIt(Paths.BepInExRootPath);
            m_Harmony.PatchAll();


            GTFO.API.LevelAPI.OnBuildDone +=  C_CullingClusterPatch.GetAllClusters;
            GTFO.API.LevelAPI.OnLevelCleanup += C_CullingClusterPatch.CleanAllClusters;

        }
        public static EntryPoint e;
        public static void GetTheSettings()
        {
            configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "mushroom.lowspecgaming.cfg"), true);



            dynamicResolution = configFile.Bind<DynamicResolution>("Setup", nameof(dynamicResolution), DynamicResolution.Stable, 
                "Scales down your resolution whenever you move your camera, will improve this in the future");
            treeDrawing = configFile.Bind<TreeDrawing>("Setup", nameof(treeDrawing), TreeDrawing.Draw, 
                "whether or not to draw IRF like trees and tentacles, this will break a lot of stuff during Kraken Fight");
            gameEnvironment = configFile.Bind<GameEnvironment>("Setup", nameof(gameEnvironment), GameEnvironment.Full, 
                "Reduce fog distance, shadow distance, no more dust particles (requires restart to take effect)");
            BioScanUpdate = configFile.Bind<BioScanBlink>("Setup", nameof(BioScanUpdate), BioScanBlink.DontBlink, 
                "Whether or not Bio Scans would blink, courtesy to McCad, this is his online code");
            hateSpitter = configFile.Bind<HateSpitter>("Setup", nameof(hateSpitter), HateSpitter.HATE, 
                "If you hate spitters, make them low quality");
            enemyBehaviourCulling = configFile.Bind<EnemyBehaviourCulling>("Setup", nameof(enemyBehaviourCulling), EnemyBehaviourCulling.Full, 
                "Reduce Enemy Update in order to save performance, will improve this feature in the future for better performance");
            textureSize = configFile.Bind<TextureSize>("Setup", nameof(textureSize), TextureSize.Full, 
                "Texture size, for the potata army");
            oneFlashLight = configFile.Bind<OneFlashLight>("Setup", nameof(oneFlashLight), OneFlashLight.All,
                "All for vanilla, use One to load only 1 and also the best flashlight for all guns (DMR)");
            redundantComponents = configFile.Bind<Experimental>("Setup", nameof(redundantComponents), Experimental.TurnOn,
                "VERY EXPERIMENTAL, turn off some redundant compenents in game || You might gain 5fps");
            currentFolderPath = configFile.Bind<string>("Setup", nameof(currentFolderPath),"" , 
                "Manual Path to the current folder that has the plugin if it fails to load normally, This will be removed soon");
            dumpTexture = configFile.Bind<bool>("Setup", nameof(dumpTexture), false,
                "Dump and extract sight textures in order to load hi res textures even at low res texture settings");
        }
        

        public static void LogIt(object data) => e.Log.LogInfo(data);
        public override bool Unload() => base.Unload();

        public static void DecompressFolder(string compressedFilePath, string decompressedFolderPath)
        {
            // Extract the contents of the ZIP archive to a folder
            ZipFile.ExtractToDirectory(compressedFilePath, decompressedFolderPath);
            // We do this when there is no folder found
            LogIt("Folder decompressed successfully.");
        }
    }

    //Enums for Game Config Settings
    //
    public enum BioScanBlink
    {
        Blink = 0,
        DontBlink = 1,
    }
    public enum GameEnvironment
    {
        Full = 0,
        Reduced = 1,
    }
    public enum TreeDrawing
    {
        Draw = 0,
        HalfDraw = 1,
        DontDraw = 2,
    }
    public enum DynamicResolution
    {
        Stable = 0,
        Dynamic = 1,
    }
    public enum HateSpitter
    {
        HATE = 0,
        LOVE = 1,
    }
    public enum EnemyBehaviourCulling
    {
        Full = 0,
        Reduced = 1,
    }
    public enum Experimental
    {
        TurnOn = 0,
        TurnOff = 1,
    }
    public enum TextureSize
    {
        Full = 0,
        Half = 1,
        Quater = 2,
        Eigth = 3,
        Low = 4,
        PentaLow = 5,
        VeryLow = 6,
        SuperLow = 7,
        UltraLow = 8,
        POTATA = 9,
        YouLiveLikeThis = 10,
    }
    public enum OneFlashLight
    { 
        All = 0,
        One = 1,
    }
    //Plugin Credits
    //It's just me lol
    //Special thanks to Flow though for helping me with this mod
    //
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "Mushroom.LowSpecGaming";

        public const string PLUGIN_NAME = "LowSpecGaming";

        public const string PLUGIN_VERSION = "0.3.0";

        public const string AUTHOR = "time1pm";

        public const string BRANCH = "beta";

        public const string INTERNAL_VERSION = "000599";
    }
}
