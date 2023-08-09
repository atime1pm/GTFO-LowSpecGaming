using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetShards;
using BepInEx;
using CullingSystem;
using Gear;
using HarmonyLib;
using ItemSetup;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.DateTimeParse;

namespace LowSpecGaming.Misc
{
    [HarmonyPatch]
    internal class WeaponPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GearMaterialFeeder), nameof(GearMaterialFeeder.ScanAndSetupParts))]

        static void MySight(GearMaterialFeeder __instance)
        {// I dont know any other ways to do this.....WEFWEFAFASFASFASF
            string[] textures = null;
            if (EntryPoint.sightPaths.TryGetValue(__instance.transform.parent.gameObject.name, out textures))
            {
                Renderer[] rens = __instance.m_allRenderers;
                int index = int.Parse(Path.GetFileNameWithoutExtension(textures[0]).Substring(0, 2));
                Material sightMat = rens.ElementAt(index).material;
                if (sightMat != null)
                {
                    string[] textureToReplace = { "_ReticuleA", "_ReticuleB", "_ReticuleC" };
                    for (int i = 0; i < textures.Length; i++)
                    {
                        Texture2D newSight = new Texture2D(sightMat.mainTexture.width, sightMat.mainTexture.height, TextureFormat.RGBA32, false);
                        byte[] data = File.ReadAllBytes(textures[i]);
                        newSight.LoadImage(data);
                        sightMat.SetTexture(Path.GetFileNameWithoutExtension(textures[i]).Substring(2), newSight);
                    }
                }
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CL_ShadowGenerator), nameof(CL_ShadowGenerator.SetCookie))]
        static void MyFlashLight(ref Texture2D cookie)
        {
            if (cookie.name.Contains("FlashlightRegularCookie"))
            {
                byte[] b = File.ReadAllBytes(EntryPoint.sightPaths["GunFlashLight"][0]);
                Texture2D newSight = new Texture2D(256, 256, TextureFormat.RGBA32, false);
                newSight.LoadImage(b);
                cookie = newSight;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_Node), nameof(C_Node.Show))]
        static bool ShowWhat()
        {//Obselete code still being called for some reasons
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_Node), nameof(C_Node.Hide))]
        static bool HideWhat()
        {//Obselete code still being called for some reasons
            return false;
        }

    }
}
