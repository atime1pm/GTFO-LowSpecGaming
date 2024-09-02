using CullingSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using LevelGeneration;
using Il2CppSystem.Data;
using InControl;
using static RootMotion.FinalIK.AimPoser;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace LowSpecGaming.Patches
{
    public static class C_CullingClusterPatch
    {
        public static List<GameObject> CombinedGameObjects = new List<GameObject>();
        public static int initial_renderer = 0;
        public static int after_renderer = 0;

        public static void GetAllClusters() 
        {
            foreach (LG_Area area in UnityEngine.Object.FindObjectsOfType<LG_Area>()) 
            {
                var node = area.m_courseNode.m_cullNode;


                if (node == null) continue;
                if (area == null) continue;
                if (area.m_courseNode == null) continue;

                EntryPoint.LogIt($"Checking Renderers at {area.name} Doors");

                foreach (var p in node.m_portals) 
                {
                    var cluster = p.TryCast<C_CullBucket>();
                    var ren_array = cluster.Renderers.ToArray();
                    var ren_list = ClusterRestruct.Restruct(ren_array);
                    cluster.Renderers.Clear();
                    foreach (var r in ren_list)
                    { 
                        cluster.Renderers.Add(r);
                    }
                }
            }

            int count = 0;
            int count2 = 0;
            foreach (var shadow in UnityEngine.Object.FindObjectsOfType<MeshRenderer>()) 
            {
                var name = shadow.name.ToLower();
                string name2 = "";
                if (name.Length > 7)
                    name2 = name.Substring(0, 6);

                if (name.Contains("shadow") || name.Contains("cube"))
                { 
                    shadow.forceRenderingOff = true;
                    shadow.enabled = false;
                    count++;
                }
                if (name2 == "c_prop")
                {
                    shadow.forceRenderingOff = true;
                    shadow.enabled = false;
                    count2++;
                }
            }
            EntryPoint.LogIt($"Disable Shadows: {count} --- S_prop: {count2}");
            EntryPoint.LogIt($"Total Initial Renderers: {initial_renderer} --- Total After Renderers: {after_renderer}");
        }
        public static void CleanAllClusters() { 
            C_CullingManager.PointLights_WantsToShow.Clear();
            C_CullingManager.SpotLights_WantsToShow.Clear();
            initial_renderer = 0;
            after_renderer = 0;
            for (int i = 0; i < CombinedGameObjects.Count; i++)
            {
                GameObject.Destroy(CombinedGameObjects[i]);
            }
            GC.Collect();
        }
       
        public static void HideAllCombined() 
        {
            for (int i = 0; i < CombinedGameObjects.Count; i++)
            {
                CombinedGameObjects[i].GetComponent<Renderer>().enabled = false;
                CombinedGameObjects[i].SetActive(false);
            }
        }
        public static void ShowAllCombined()
        {
            for (int i = 0; i < CombinedGameObjects.Count; i++)
            {
                CombinedGameObjects[i].GetComponent<Renderer>().enabled = true;
                CombinedGameObjects[i].SetActive(true);
            }
        }
    }
    
}
