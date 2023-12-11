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
using AssetShards;
using ItemSetup;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class SightPatch
    {
        public static byte[] flashLightData;
        public static Dictionary<string, string[]> sightPaths;
        public static Dictionary<string, string> flashlights;

        public static void GetSightFolders()
        {
            flashlights = new();
            string[] sightFolder = null;
            var FolderPath = Paths.BepInExRootPath + "\\LowSpec\\";


            if (!Directory.Exists(FolderPath))  return;
            if (EntryPoint.dumpTexture.Value)   return;

            LogIt("Trying to find sights");

            //Find the Plugin folder Automatically if it's not manually path
            if (currentFolderPath.Value.Equals("") || currentFolderPath.Value == null)
            {
                LogIt("Auto loaded and found sights");
            }
            //Manual pathing
            else
            {
                LogIt("Manually Loaded");
                FolderPath = currentFolderPath.Value;
            }

            sightFolder = Directory.GetDirectories(FolderPath);

            if (sightFolder == null) return;

            foreach (string gearSight in sightFolder)
            {
                string currentGear = gearSight.Split("\\")[gearSight.Split("\\").Count<String>() - 1];
                sightPaths[currentGear] = Directory.GetFiles(gearSight);
            }

            foreach (string l in sightPaths["FlashLights"]) {
                string flashLightName = l.Split("\\")[l.Split("\\").Count<String>() - 1];
                flashLightName = flashLightName[..^4];
                flashlights[flashLightName] = l;
            }
        }


        //Grab the paths from dict and set the texture in Weapon Mat
        //
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GearMaterialFeeder), nameof(GearMaterialFeeder.ScanAndSetupParts))]
        static void MySight(GearMaterialFeeder __instance)
        {// I dont know any other ways to do this.....WEFWEFAFASFASFASF
            if (dumpTexture.Value)   
                DumpTexture(__instance);
            else                        
                SetTexture(__instance);
        }
        static void DumpTexture(GearMaterialFeeder __instance)
        {
            for (int i = 0; i < __instance.m_allRenderers.Count; i++)
            {
                var ren = __instance.m_allRenderers.ElementAt<Renderer>(i);
                if (ren.material.shader.name.ToLower().Contains("unlit"))
                {
                    string[] textNames = { "_MainTex", "_ReticuleA", "_ReticuleB", "_ReticuleC" };
                    foreach (var name in textNames)
                    {
                        SaveTexture(__instance.transform.parent.gameObject.name, i, ren.material, name);
                    }
                    LogIt(__instance.transform.parent.gameObject.name + " Dumped");
                    break;
                }
            }
        }
        static void SaveTexture(string weaponName,int index,Material mat,string TextName) {
            try
            {
                var savedSight = mat.GetTexture(TextName).Cast<Texture2D>();

                var tmp = RenderTexture.GetTemporary(savedSight.width, savedSight.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                var previous = RenderTexture.active;
                Graphics.Blit(savedSight, tmp);
                RenderTexture.active = previous;
                Texture2D newText = new (savedSight.width, savedSight.height);
                previous = RenderTexture.active;
                RenderTexture.active = tmp;

                newText.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                newText.Apply();
                RenderTexture.ReleaseTemporary(tmp);
                byte[] f = newText.EncodeToPNG();

                string fileName = "";
                if (index < 10)
                    fileName ="0"+ index + TextName;
                else
                    fileName =index+TextName;

                var FolderPath = Paths.BepInExRootPath + "\\LowSpec\\";
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);


                if (!Directory.Exists(FolderPath + weaponName))
                    Directory.CreateDirectory(FolderPath + weaponName);

                File.WriteAllBytes(FolderPath+weaponName+"\\"+fileName + ".png", f);
            }
            catch { LogIt("Skipping Texture"); }
        }
        static void SetTexture(GearMaterialFeeder __instance) {
            if (!sightPaths.TryGetValue(__instance.transform.parent.gameObject.name, out string[] textures)) return;
            Material sightMat = null;
            try
            {
                sightMat = __instance.m_allRenderers.
                    ElementAt(int.Parse(Path.GetFileNameWithoutExtension(textures[0])[..2])).material;
            }
            catch {LogIt("why...");}

            if (sightMat == null) return;

            foreach (var tex in textures)
            {
                try
                {
                    Texture2D newSight = new(512, 512, TextureFormat.RGBA32, false);

                    newSight.LoadImage(File.ReadAllBytes(tex));
                    sightMat.SetTexture(Path.GetFileNameWithoutExtension(tex)[2..], newSight);
                }
                catch { LogIt("Couldnt find texture for " + __instance.transform.parent.gameObject.name); }
            }
            LogIt(__instance.transform.parent.gameObject.name + " Texture Set");
        }
        //Set the Flashlight Texture
        //it's a byte[] so it shouldn't be downscaled
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CL_ShadowGenerator), nameof(CL_ShadowGenerator.SetCookie))]
        static void MyFlashLight(ref Texture2D cookie)
        {
            if (cookie.name.Contains("Flashlight"))
            {
                Texture2D newSight = new(512, 512, TextureFormat.RGBA32, false);
                newSight.LoadImage(File.ReadAllBytes(flashlights[cookie.name]));
                cookie = newSight;
            }
        }

    }
}
