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

namespace LowSpecGaming.Patches
{
    public class C_CullingClusterPatch : MonoBehaviour
    {
        public static Dictionary<Vector3, ClusterStruct> clusters;
        public static Vector3 currentPos = new Vector3(0, 0, 0);
        public static Vector3 prevPos = new Vector3(0, 0, 0);
        private static readonly float cullSize = 18.0f;
        public static C_CullingClusterPatch current;
        public static ClusterStruct[] currentPat = new ClusterStruct[0];
        public static ClusterStruct[] hidePat = new ClusterStruct[0];
        public static List<GameObject> CombinedGameObjects = new List<GameObject>();
        public static int initialRenderersCount = 0;
        public static int afterRenderersCount = 0;
        public C_Camera c_cam;
        public static float time;
        public void Start()
        {
            current = this;
            c_cam = gameObject.GetComponent<C_Camera>();
            this.enabled = false;
            time = 0;
        }

        public static void EnableInLevel() 
        {
            current.enabled = true;
        }
        public static void GetAllClusters() 
        {
            clusters = new Dictionary<Vector3, ClusterStruct>();
            clusters[new Vector3(0,0,0)] = new ClusterStruct();
            foreach (LG_Area area in UnityEngine.Object.FindObjectsOfType<LG_Area>()) 
            {
                var node = area.m_courseNode.m_cullNode;


                if (node == null) continue;
                if (area == null) continue;
                if (area.m_courseNode == null) continue;


                EntryPoint.LogIt($"Adding Renderers at {area.name}");
                foreach (var c in node.m_staticCullables)
                {
                    var cluster = c.TryCast<C_CullingCluster>();
                    if (cluster != null)
                        AddRendererToCluster(cluster.Renderers);
                }


                foreach(var c in node.SpecialRenderableObjects)
                    if (c.CullBucket != null) c.CullBucket.Show();

                foreach (var p in node.m_portals)
                {
                    var cluster = p.TryCast<C_CullBucket>();
                    if (cluster != null)
                        AddRendererToCluster(cluster.Renderers);
                }
                foreach (var g in area.m_gates) 
                    AddRendererToCluster(g.CullBucket.Renderers);

                foreach (var m in area.GetComponentsInChildren<LG_DoorBladeCuller>()) 
                    AddRendererToCluster(m.CullBucket.Renderers);
            }

            foreach (var item in UnityEngine.Object.FindObjectsOfType<ItemCuller>())
            { 
                item.Show();
                item.CullBucket.Show();
            }
            EntryPoint.LogIt("Restructuring clusters");
            foreach (var key in clusters.Keys)
                clusters[key].Restruct(key);

            GC.Collect();

            EntryPoint.LogIt("Linking clusters");
            foreach (var key in clusters.Keys)
                clusters[key].Link(key);


            GC.Collect();
            EntryPoint.LogIt($"TOTAL Intial Renderers Count: {initialRenderersCount} >>> {afterRenderersCount}");

            ForceCull(clusters.ElementAt(0).Key);
            ForceCullAtCurrentPos();
        }
        public static void AddRendererToCluster(Il2CppReferenceArray<Renderer> renderers) 
        {
            foreach (var r in renderers) 
            {
                var key = VectorRoundFlat(r.bounds.center);
                if (clusters.TryGetValue(key, out ClusterStruct rList))
                {
                    rList.ren[r.GetInstanceID()] = r;
                    rList.ren[r.GetInstanceID()].enabled = false;
                }
                else
                {
                    clusters[key] = new();
                    clusters[key].ren[r.GetInstanceID()] = r;
                    clusters[key].ren[r.GetInstanceID()].enabled = false;

                }
            }
        }
        public static void AddRendererToCluster(Il2CppSystem.Collections.Generic.List<Renderer> renderers)
        {
            foreach (var r in renderers)
            {
                var key = VectorRoundFlat(r.bounds.center);
                if (clusters.TryGetValue(key, out ClusterStruct rList))
                {
                    rList.ren[r.GetInstanceID()] = r;
                    rList.ren[r.GetInstanceID()].enabled = false;
                }
                else
                {
                    clusters[key] = new();
                    clusters[key].ren[r.GetInstanceID()] = r;
                    clusters[key].ren[r.GetInstanceID()].enabled = false;
                }
            }
        }

        public static void CleanAllClusters() { 
            clusters = new Dictionary<Vector3, ClusterStruct>();
            C_CullingManager.PointLights_WantsToShow.Clear();
            C_CullingManager.SpotLights_WantsToShow.Clear();
            for (int i = 0; i < CombinedGameObjects.Count; i++)
            {
                GameObject.Destroy(CombinedGameObjects[i]);
            }
            initialRenderersCount = 0;
            afterRenderersCount = 0;

            GC.Collect();
        }

        public void FixedUpdate()
        {
            Pattern_Cull(transform.position);
        }
        public static void ForceCull(Vector3 pos) 
        {
            pos = VectorRoundFlat(pos);
            var currentPat = clusters[pos].linkedPattern;
            for (var i = 0; i < currentPat.Length; i++)
            {
                currentPat[i].ShowRenderer();
            }
        }
        public static void ForceCullAtCurrentPos()
        {
            prevPos = VectorRoundFlat(current.transform.position);
            var currentPat = clusters[VectorRoundFlat(current.transform.position)].linkedPattern;
            for (var i = 0; i < currentPat.Length; i++)
            {
                currentPat[i].ShowRenderer();
            }
        }
        public static void Pattern_Cull(Vector3 pos) 
        {
            currentPos = VectorRoundFlat(pos);
            if (currentPos != prevPos)
            {
                currentPat = clusters[currentPos].linkedPattern;
                hidePat = currentPat.Except(clusters[prevPos].linkedPattern).ToArray();

                for (var i = 0; i < hidePat.Length; i++)
                    hidePat[i].HideRenderer();
                for (var i = 0; i < currentPat.Length; i++)
                    currentPat[i].ShowRenderer();
            }
            prevPos = currentPos;
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
        public static Vector3 VectorRound(Vector3 inp)
        {
            return new(Mathf.RoundToInt(inp.x / cullSize), Mathf.RoundToInt(inp.y / cullSize), Mathf.RoundToInt(inp.z / cullSize));
        }
        public static Vector3 VectorRoundFlat(Vector3 inp)
        {
            return new(Mathf.RoundToInt(inp.x / cullSize), 0, Mathf.RoundToInt(inp.z / cullSize));
        }
    }
    
}
