using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using LowSpecGaming.Misc;
using static LowSpecGaming.EntryPoint;
using LowSpecGaming;
namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class SightPatch
    {
        public static byte[] flashLightData;
        public static Dictionary<string, string[]> sightPaths;

        public static void GetSightFolders()
        {
            string pluginFolder = "";//The folder 
            string[] sightFolder = null;

            LogIt("Trying to find sights");


            //Find the Plugin folder Automatically if it's not manually path
            if (currentFolderPath.Value.Equals("") || currentFolderPath.Value == null)
            {
                LogIt("Auto Loaded");
                foreach (string folder in Directory.GetDirectories(Paths.PluginPath))
                    if (folder.Contains(PluginInfo.PLUGIN_NAME)) 
                        pluginFolder = folder;
            }
            //Manual pathing
            else
            {
                LogIt("Manually Loaded");
                pluginFolder = currentFolderPath.Value;
            }

            //Check if the sight folder is there or not
            //If not then decompress the zip Sight.gtfo 
            if (!(Directory.Exists(Path.Combine(pluginFolder, "Sight"))))
            {
                try { DecompressFolder(Path.Combine(pluginFolder, "Sight.gtfo"), pluginFolder); } 
                catch { LogIt("Couldn't Find Sight.gtfo --- Something is very wrong here."); }
            }

            LogIt("Found Sights");

            sightFolder = Directory.GetDirectories(Path.Combine(pluginFolder, "Sight"));

            if (sightFolder == null) return;

            foreach (string gearSight in sightFolder)
            {
                //There's a problem with Sniper Rifle not being correct due to the font
                //being compressed in the zip file
                //
                if (gearSight.ToLower().Contains("pr 11"))
                {
                    string newSightFolder = gearSight[..21];
                    newSightFolder += "GearItem_Köning PR 11";
                    if (gearSight != newSightFolder)
                    {
                        Directory.Move(gearSight, newSightFolder);
                        LogIt("Sniper fixed, it was the compression's fault");
                    }
                    //Put it in a dict as GearName -- Texture Paths
                    string currentGear = newSightFolder.Split("\\")[newSightFolder.Split("\\").Count<String>() - 1];
                    sightPaths[currentGear] = Directory.GetFiles(newSightFolder);
                }
                else
                {
                    string currentGear = gearSight.Split("\\")[gearSight.Split("\\").Count<String>() - 1];
                    sightPaths[currentGear] = Directory.GetFiles(gearSight);
                }
            }
            //Load the FlashLights as bytes so it doesn't get downscale by the game
            flashLightData = File.ReadAllBytes(sightPaths["GunFlashLight"][0]);
        }


        //Grab the paths from dict and set the texture in Weapon Mat
        //
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GearMaterialFeeder), nameof(GearMaterialFeeder.ScanAndSetupParts))]
        static void MySight(GearMaterialFeeder __instance)
        {// I dont know any other ways to do this.....WEFWEFAFASFASFASF
            if (!sightPaths.TryGetValue(__instance.transform.parent.gameObject.name, out string[] textures)) return;

            Material sightMat = __instance.m_allRenderers.
                ElementAt(int.Parse(Path.GetFileNameWithoutExtension(textures[0])[..2])).material;

            if (sightMat == null) return;

            foreach (var tex in textures)
            {
                try
                {
                    Texture2D newSight = new(512, 512, TextureFormat.RGBA32, false);
                    newSight.LoadImage(File.ReadAllBytes(tex));
                    sightMat.SetTexture(Path.GetFileNameWithoutExtension(tex)[2..], newSight);
                }

                catch { EntryPoint.LogIt("Couldnt find texture for " + __instance.transform.parent.gameObject.name); }
            }
        }



        //Set the Flashlight Texture
        //it's a byte[] so it shouldn't be downscaled
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CL_ShadowGenerator), nameof(CL_ShadowGenerator.SetCookie))]
        static void MyFlashLight(ref Texture2D cookie)
        {
            if (cookie.name.Contains("FlashlightRegularCookie"))
                cookie.LoadImage(flashLightData);
        }

    }
}
