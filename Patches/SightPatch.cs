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
using System.Drawing;
using System.IO;
using System.Reflection;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class SightPatch
    {
        public static byte[] flashLightData;
        public static Dictionary<string, string[]> sightPaths;
        public static Dictionary<string, string> flashlights;
        public static UnityEngine.Color newColor = new (10f,0.5f,0f);
        private static string[] textNames = { "_MainTex", "_ReticuleA", "_ReticuleB", "_ReticuleC" };
        public static void GetSightFolders()
        {
            flashlights = new();
            string[] sightFolder = null;
            var FolderPath = Paths.BepInExRootPath + "\\LowSpec\\";


            if (!Directory.Exists(FolderPath))  return;
            if (dumpTexture.Value)   return;

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
            try {
                foreach (string gearSight in sightFolder)
                {
                    string currentGear = gearSight.Split("\\")[gearSight.Split("\\").Count<String>() - 1];
                    sightPaths[currentGear] = Directory.GetFiles(gearSight);
                }
            }
            catch { LogIt("NO GEAR SIGHT FOUND, MOVING ON"); }
            try
            {
                foreach (string l in sightPaths["FlashLights"])
                {
                    string flashLightName = l.Split("\\")[l.Split("\\").Count<String>() - 1];
                    flashLightName = flashLightName[..^4];
                    flashlights[flashLightName] = l;
                }
            }
            catch { LogIt("NO FLASHLIGHT FOUND, MOVING ON"); }
        }


        //Grab the paths from dict and set the texture in Weapon Mat
        //
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GearMaterialFeeder), nameof(GearMaterialFeeder.ScanAndSetupParts))]
        static void MySight(GearMaterialFeeder __instance)
        {// I dont know any other ways to do this.....WEFWEFAFASFASFASF
            try
            {
                if (dumpTexture.Value)
                    DumpTexture(__instance);
                else
                    SetCustomTexture(__instance);
            }
            catch { LogIt("Skipping texture for"+ __instance.transform.parent.gameObject.name + ".."); }
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
            var savedSight = mat.GetTexture(TextName).Cast<Texture2D>();

            var tmp = RenderTexture.GetTemporary(savedSight.width, savedSight.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            var previous = RenderTexture.active;
            Graphics.Blit(savedSight, tmp);
            RenderTexture.active = previous;
            Texture2D newText = new(savedSight.width, savedSight.height);
            previous = RenderTexture.active;
            RenderTexture.active = tmp;
            
            newText.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            newText.Apply();
            if (TextName == "_MainTex")
            {
                UnityEngine.Color ogColor;
                for (int x = 0; x < newText.width; x++)
                {
                    for (int y = 0; y < newText.height; y++)
                    {
                        ogColor = newText.GetPixel(x, y) * newColor;
                        newText.SetPixel(x, y, ogColor);
                    }
                }
                newText.Apply();
            }
            RenderTexture.ReleaseTemporary(tmp);

            byte[] f = newText.EncodeToPNG();

            string fileName = "";
            if (index < 10)
                fileName = "0" + index + TextName;
            else
                fileName = index + TextName;

            var FolderPath = Paths.BepInExRootPath + "\\LowSpec\\";
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);


            if (!Directory.Exists(FolderPath + weaponName))
                Directory.CreateDirectory(FolderPath + weaponName);

            File.WriteAllBytes(FolderPath + weaponName + "\\" + fileName + ".png", f);
        }
        static void SetVanillaTexture(GearMaterialFeeder __instance) {
            for (int i = 0; i < __instance.m_allRenderers.Count; i++)
            {
                var ren = __instance.m_allRenderers.ElementAt<Renderer>(i);
                if (ren.material.shader.name.ToLower().Contains("unlit"))
                {
                    var sightMat = ren.material;
                    foreach (var name in textNames)
                    {
                        try
                        {
                            sightMat.SetTexture(name, LoadEmbeddedTexture(__instance.transform.parent.gameObject.name, name));
                            sightMat.SetFloat("_SightDirt", 0);
                        }
                        catch {
                            EntryPoint.LogIt($"Skipping texture {name} for {__instance.transform.parent.gameObject.name}");
                        }
                    }
                    break;
                }
            }
        }
        static void SetCustomTexture(GearMaterialFeeder __instance) {
            if (!sightPaths.TryGetValue(__instance.transform.parent.gameObject.name, out string[] textures)) {
                SetVanillaTexture(__instance);
                return;
            }

            Material sightMat = null;
            sightMat = __instance.m_allRenderers.
                    ElementAt(int.Parse(Path.GetFileNameWithoutExtension(textures[0])[..2])).material;

            if (sightMat == null) return;

            foreach (var tex in textures)
            {
                Texture2D newSight = new(1024, 1024, TextureFormat.RGBA32, false);
                newSight.LoadImage(File.ReadAllBytes(tex));
                newSight.Apply();
                sightMat.SetTexture(Path.GetFileNameWithoutExtension(tex)[2..], newSight);
                sightMat.SetFloat("_SightDirt", 0);
            }
        }
        //Set the Flashlight Texture
        //it's a byte[] so it shouldn't be downscaled
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CL_ShadowGenerator), nameof(CL_ShadowGenerator.SetCookie))]
        static void MyFlashLight(ref Texture2D cookie)
        {
            if (cookie.name.Contains("FlashlightRegular"))
            {
                try
                {
                    if (oneFlashLight.Value == OneFlashLight.All)
                    {
                        if (flashlights.TryGetValue(cookie.name, out string newFlashTexture))
                        {
                            Texture2D newSight = new(512, 512, TextureFormat.RGBA32, false);
                            newSight.LoadImage(File.ReadAllBytes(newFlashTexture));
                            cookie = newSight;
                        }
                        else
                        {
                            cookie = TryLoadEmbeddedTexture($"LowSpecGaming.Resources.FlashLights.{cookie.name}.png");
                        }
                    }
                    else 
                    {
                        cookie = TryLoadEmbeddedTexture($"LowSpecGaming.Resources.FlashLights.FlashlightRegularCookie_01.png");
                    }
                }
                catch { }
            }
        }
        public static Texture2D LoadEmbeddedTexture(string gearName, string texName) {
            Texture2D embTex2D = new(512, 512, TextureFormat.RGBA32, false);
            embTex2D.LoadImage(GetEmbededResources(gearName, texName));
            embTex2D.Apply();
            return embTex2D;
        }
        public static Texture2D TryLoadEmbeddedTexture(string resourceName) {
            Texture2D embTex2D = new(512, 512, TextureFormat.RGBA32, false);
            embTex2D.LoadImage(TryGetEmbededResources(resourceName));
            embTex2D.Apply();
            return embTex2D;
        }
        static byte[] GetEmbededResources(string gearName,string texName) {
            var assembly = Assembly.GetExecutingAssembly();
            gearName = gearName.Replace(" ", "_");
            var resourcePath = $"LowSpecGaming.Resources.{gearName}.{texName}.png";
        
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found", resourcePath);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        static byte[] TryGetEmbededResources(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream( resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found",  resourceName);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
        private static void ListResourcesInAssembly()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly is null)
                return;

            var resources = assembly.GetManifestResourceNames();
            if (resources.Length == 0)
                return;

            EntryPoint.LogIt($"Resources in {assembly.FullName}");
            foreach (var resource in resources)
            {
                EntryPoint.LogIt(resource);
            }

            EntryPoint.LogIt("");
        }
    }
}
