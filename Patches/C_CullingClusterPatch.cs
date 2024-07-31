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
        public static float cullSize = 13.0f;
        public static C_CullingClusterPatch current;
        public static List<C_Light> lightList = new();
        public static List<C_LightDynamic> c_LightDynamics = new();
        public static Vector3[] currentPat = new Vector3[0];
        public static Vector3[] hidePat = new Vector3[0];
        public void Awake()
        {
            current = this;
            this.enabled = false;
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
                EntryPoint.LogIt("Adding");
                foreach (var c in node.m_staticCullables)
                {
                    var cluster = c.TryCast<C_CullingCluster>();
                    if (cluster == null) continue;
                    AddRendererToCluster(cluster.Renderers);
                }
                foreach(var c in node.SpecialRenderableObjects)
                {
                    Vector3 key;
                    key = VectorRoundFlat(c.GetBounds().center);
                    if (!clusters.TryGetValue(key, out ClusterStruct specList))
                        clusters[key] = new();
                    clusters[key].spec.Add(c);

                    var b = c.CullBucket;
                    if (b == null) continue;
                    AddRendererToCluster(b.Renderers);
                    b.Show();
                }
                foreach (var p in node.m_portals)
                {
                    var cluster = p.TryCast<C_CullBucket>();
                    if (cluster == null) continue;
                    AddRendererToCluster(cluster.Renderers);
                }
                foreach (var g in area.m_gates) 
                {
                    AddRendererToCluster(g.CullBucket.Renderers);
                }
                foreach (var m in area.GetComponentsInChildren<LG_DoorBladeCuller>()) 
                {
                    AddRendererToCluster(m.CullBucket.Renderers);
                }
            }

            foreach (var item in UnityEngine.Object.FindObjectsOfType<ItemCuller>())
            { 
                item.Show();
                item.CullBucket.Show();
            }

            foreach (var item in lightList)
                AddLightToCluster(item);



            foreach (var key in clusters.Keys)
                clusters[key].Restruct();

            EntryPoint.LogIt("Restruct");
            GC.Collect();
            foreach (var key in clusters.Keys)
                clusters[key].Link(key);
            EntryPoint.LogIt("Link");
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
        public static void AddLightToCluster(C_Light light)
        {
            var key = VectorRoundFlat(light.Bounds.center);
            if (clusters.TryGetValue(key, out ClusterStruct rList))
            {
                rList.lightList.Add(light);
                light.Hide();
            }
            else
            {
                clusters[key] = new();
                clusters[key].lightList.Add(light);
                light.Hide();

            }
        }
        public static void CleanAllClusters() { 
            clusters = new Dictionary<Vector3, ClusterStruct>();
            lightList = new();
        }

        public void FixedUpdate() 
        {

        }
        public void Update()
        {
            C_CullingManager.PointLights_WantsToShow.Clear();
            C_CullingManager.SpotLights_WantsToShow.Clear();

            for(int i = 0;  i < c_LightDynamics.Count; i++) 
            {
                if (c_LightDynamics[i].m_clusterLight.m_isOn)
                    c_LightDynamics[i].Show();
                else
                    c_LightDynamics[i].Hide();
            }
            Pattern_Cull(transform.position);
            C_CullingManager.SortAndUpdateLights();

        }

        public static void UpdateLight() 
        {
            for (int index = 0; index < C_CullingManager.SpotLights_WantsToShow.Count; ++index)
                C_CullingManager.SpotLights_WantsToShow[index].UpdateVisible();
            for (int index = 0; index < C_CullingManager.PointLights_WantsToShow.Count; ++index)
                C_CullingManager.PointLights_WantsToShow[index].UpdateVisible();
        }
        public void UpdateDynamicLight() 
        {
            for (int i = 0; i < c_LightDynamics.Count; i++)
            {
                c_LightDynamics[i].UpdateVisible();
            }
        }

        public static void ForceCull(Vector3 pos) 
        {
            pos = VectorRoundFlat(pos);
            var currentPat = clusters[pos].linkedPattern;
            for (var i = 0; i < currentPat.Length; i++)
            {
                clusters[currentPat[i]].ShowRenderer();
            }
        }
        public static void ForceCullAtCurrentPos()
        {
            prevPos = VectorRoundFlat(current.transform.position);
            var currentPat = clusters[VectorRoundFlat(current.transform.position)].linkedPattern;
            for (var i = 0; i < currentPat.Length; i++)
            {
                clusters[currentPat[i]].ShowRenderer();
            }
        }
        public static void Pattern_Cull(Vector3 pos) 
        {
            currentPos = VectorRoundFlat(pos);
            if (currentPos != prevPos)
            {

            }
            currentPat = clusters[currentPos].linkedPattern;
            hidePat = currentPat.Except(clusters[prevPos].linkedPattern).ToArray();
            for (var i = 0; i < hidePat.Length; i++)
            {
                clusters[hidePat[i]].HideRenderer();
            }
            for (var i = 0; i < currentPat.Length; i++)
            {
                clusters[currentPat[i]].ShowRenderer();
            }
            prevPos = currentPos;
        }

        public static Vector3 VectorRound(Vector3 inp)
        {
            Vector3 outp = new (Mathf.RoundToInt(inp.x / cullSize), Mathf.RoundToInt(inp.y / cullSize), Mathf.RoundToInt(inp.z / cullSize));
            return outp;
        }
        public static Vector3 VectorRoundFlat(Vector3 inp)
        {
            Vector3 outp = new(Mathf.RoundToInt(inp.x / cullSize), 0, Mathf.RoundToInt(inp.z / cullSize));
            return outp;
        }

        
    }
    public class ClusterStruct
    {
        public bool isShown;
        public Dictionary<int,Renderer> ren;
        public Renderer[] renList;
        public int listCount;
        public Vector3[] linkedPattern;
        public List<C_SpecialRenderableObject> spec;
        public List<C_Light> lightList;
        public C_Light[] lightArray;
        public int lightCount;
        public ClusterStruct() 
        {
            this.isShown = false;
            this.ren = new Dictionary<int, Renderer>();
            this.spec = new List<C_SpecialRenderableObject>();
            this.lightList = new List<C_Light>();
        }
        public void Restruct()
        {
            renList = ren.Select(x => x.Value).ToArray();
            this.ren.Clear();
            listCount = renList.Length;

            lightArray = lightList.ToArray();
            lightList.Clear();
            lightCount = lightArray.Length;
            EntryPoint.LogIt($"{lightCount}");
        }
        public void Link(Vector3 pos)
        {
            var pat = GetPattern(pos);
            var tempLink = new List<Vector3>();
            foreach (var p in pat)
            {
                try 
                {
                    var _ = C_CullingClusterPatch.clusters[p];
                    tempLink.Add(p); 
                }
                catch { }
            }
            linkedPattern = tempLink.ToArray();
        }

        public void ShowRenderer()
        {
            for (int i = 0; i < lightCount; i++)
            {
                lightArray[i].Show();
            }
            if (isShown) return;
            for (int i = 0; i < listCount; i++)
            {
                renList[i].enabled = true;
            }

            isShown = true;
        }
        public void HideRenderer()
        {
            if (!isShown) return;
            for (int i = 0; i < listCount; i++)
            {
                renList[i].enabled = false;
            }

            isShown = false;
        }
        public static Vector3[] GetPattern(Vector3 pos)
        {
            return new Vector3[]
            { 
                // TOP UPPER
                pos + new Vector3(-1,0,2),       //       #00        
                pos + new Vector3(0, 0,2),       //       0#0        
                pos + new Vector3(1, 0,2),       //       00#        

                // MID UPPER
                pos + new Vector3(-2,0,1),       //      #0000        
                pos + new Vector3(-1,0,1),       //      0#000        
                pos + new Vector3(0, 0,1),       //      00#00        
                pos + new Vector3(1, 0,1),       //      000#0       
                pos + new Vector3(2, 0,1),       //      0000#       

                // MID 
                pos + new Vector3(-2,0,0),       //      #0000        
                pos + new Vector3(-1,0,0),       //      0#000        
                pos + new Vector3(0, 0,0),       //      00#00        
                pos + new Vector3(1, 0,0),       //      000#0       
                pos + new Vector3(2, 0,0),       //      0000#    

                // MID BOT
                pos + new Vector3(-2,0,-1),       //      #0000        
                pos + new Vector3(-1,0,-1),       //      0#000        
                pos + new Vector3(0, 0,-1),       //      00#00        
                pos + new Vector3(1, 0,-1),       //      000#0       
                pos + new Vector3(2, 0,-1),       //      0000#    

                // TOP BOT
                pos + new Vector3(-1,0,-2),       //       #00        
                pos + new Vector3(0, 0,-2),       //       0#0        
                pos + new Vector3(1, 0,-2),       //       00#    
            };

        }
    }
}
