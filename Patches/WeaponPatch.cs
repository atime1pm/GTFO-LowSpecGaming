using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AssetShards;
using BepInEx;
using CullingSystem;
using Gear;
using HarmonyLib;
using ItemSetup;
using LowSpecGaming.Structs;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.DateTimeParse;

namespace LowSpecGaming.Misc
{
    [HarmonyPatch]
    internal class WeaponPatch
    {
        static GameObject emptyShell = new GameObject("EmptyShell");
        public static HiTexture flashlight = new HiTexture(EntryPoint.sightPaths["GunFlashLight"][0]);
        public static byte[] data;
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
                cookie.LoadImage(data);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShellCasing), nameof(ShellCasing.Awake))]
        static bool MyMags(ShellCasing __instance)
        {
            __instance.gameObject.active = false;
            GameObject.Destroy(__instance.transform.GetChild(0).gameObject);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DisableAfterDelay), nameof(DisableAfterDelay.Awake))]
        static void MyBullets(DisableAfterDelay __instance)
        {
            __instance.delay = 0;
        }
    }
}
