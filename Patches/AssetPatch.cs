using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetShards;
using BepInEx.Unity.IL2CPP.Hook;
using ChainedPuzzles;
using CullingSystem;
using GameData;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class AssetPatch
    {
        public static ComplexResourceSetDataBlock current;
        public static List<GameObject> objects;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ComplexResourceSetDataBlock), nameof(ComplexResourceSetDataBlock.PopulateFullResourceArrays))]
        public static void AssetPatching(ComplexResourceSetDataBlock __instance)
        {
            current = __instance;

            EntryPoint.LogIt("Asset Loading");
        }
        public static void GetThePrefabs() 
        {
            return;
            objects = new List<GameObject>();
            var o = Resources.FindObjectsOfTypeAll(Il2CppType.Of<GameObject>());
            for (int i = o.Length - 1;i> 0;i--)
            {
                if (o[i].name.Contains("c_prop"))
                {
                    var prop = o[i].TryCast<GameObject>();
                    objects.Add(prop);
                    GameObject.Destroy(prop.GetComponent<MeshFilter>());
                    GameObject.Destroy(prop.GetComponent<MeshRenderer>());

                }
            }
            EntryPoint.LogIt(objects.Count);
        }
    }
}
