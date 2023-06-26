using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using Gear;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.DateTimeParse;

namespace Octomizer
{
    [HarmonyPatch]
    internal class WeaponPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GearMaterialFeeder), nameof(GearMaterialFeeder.ScanAndSetupParts))]

        static void MySight(GearMaterialFeeder __instance)
        {// I dont know any other ways to do this.....WEFWEFAFASFASFASF
            string[] textures = null;
            if (EntryPoint.sightPaths.TryGetValue(__instance.transform.parent.gameObject.name,out textures))
            {
                EntryPoint.entry.Log.LogInfo(__instance.transform.parent.gameObject.name);
                Renderer[] rens = __instance.m_allRenderers;
                int index = int.Parse((Path.GetFileNameWithoutExtension(textures[0])).Substring(0, 2));
                Material sightMat = rens.ElementAt<Renderer>(index).material;
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
    }
}
