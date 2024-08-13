using LowSpecGaming.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    public class ClusterStruct
    {
        public bool isShown;
        public Dictionary<int, Renderer> ren;
        public Renderer[] renList;
        public int listCount;
        public ClusterStruct[] linkedPattern;
        public ClusterStruct()
        {
            this.isShown = false;
            this.ren = new Dictionary<int, Renderer>();
        }
        public void Restruct(Vector3 key)
        {
            var tempRenList = ren.Select(x => x.Value).ToList();

            if (tempRenList.Count < 1)
            { 
                renList = tempRenList.ToArray();
                listCount = 0;
            }
            var beforeCount = tempRenList.Count;
            Dictionary<string,List<Renderer>> duplicateRenderers = new Dictionary<string,List<Renderer>>();
            for (int i = 0; i < tempRenList.Count; i++)
            {
                var name = tempRenList[i].gameObject.name;
                if (!duplicateRenderers.TryGetValue(name,out List<Renderer> _)) 
                {
                    duplicateRenderers[name] = new List<Renderer>();
                }
                duplicateRenderers[name].Add(tempRenList[i]);
            }
            
            var dupRenlist = new List<Renderer>();
            var dupKey = new List<string>();
            foreach (var dup in duplicateRenderers.Keys)
            {
                dupKey.Add(dup);
            }
            foreach (var dup in dupKey) 
            {
                if (dup.ToLower().Contains("lock"))
                {
                    dupRenlist.AddRange(duplicateRenderers[dup]);
                    continue;
                }
                try
                {
                    dupRenlist.AddRange(C_Combine.CombineMeshes(duplicateRenderers[dup]));
                }
                catch (Exception ex)
                {
                    dupRenlist.AddRange(duplicateRenderers[dup]);
                }
            }
            C_CullingClusterPatch.initialRenderersCount += beforeCount;
            C_CullingClusterPatch.afterRenderersCount += dupRenlist.Count;
            EntryPoint.LogIt($"Cluster at {key} Renderer Count: {beforeCount} > {dupRenlist.Count}");
            renList = dupRenlist.ToArray();
            tempRenList.Clear();
            dupRenlist.Clear();
            this.ren.Clear();
            listCount = renList.Length;
        }
        public void Link(Vector3 pos)
        {
            var tempLink = new List<ClusterStruct>();
            foreach (var p in GetPattern(pos))
            {
                try   {tempLink.Add(C_CullingClusterPatch.clusters[p]);}
                catch { }
            }
            linkedPattern = tempLink.ToArray();
        }

        public void ShowRenderer()
        {
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
