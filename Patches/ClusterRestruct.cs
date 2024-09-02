using LowSpecGaming.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    public static class ClusterRestruct
    {
        public static Renderer[] Restruct(Renderer[] initialRenderers)
        {
            return Restruct(initialRenderers.ToList());
        }
        public static Renderer[] Restruct(List<Renderer> initialRenderers)
        {
            if (initialRenderers.Count < 1)
            {
                return initialRenderers.ToArray();
            }
            var beforeCount = initialRenderers.Count;
            Dictionary<string,List<Renderer>> duplicateRenderers = new Dictionary<string,List<Renderer>>();
            for (int i = 0; i < initialRenderers.Count; i++)
            {
                var name = initialRenderers[i].gameObject.name;
                if (!duplicateRenderers.TryGetValue(name,out List<Renderer> _)) 
                {
                    duplicateRenderers[name] = new List<Renderer>();
                }
                duplicateRenderers[name].Add(initialRenderers[i]);
            }
            
            var dupRenlist = new List<Renderer>();
            var dupKey = new List<string>();
            foreach (var dup in duplicateRenderers.Keys)
                dupKey.Add(dup);

            foreach (var dup in dupKey) 
            {
                if (dup.ToLower().Contains("lock"))
                {
                    dupRenlist.AddRange(duplicateRenderers[dup]);
                    continue;
                }
                try     { dupRenlist.AddRange(C_Combine.CombineMeshes(duplicateRenderers[dup])); }
                catch   {dupRenlist.AddRange(duplicateRenderers[dup]);}
            }
            C_CullingClusterPatch.initial_renderer += beforeCount;
            C_CullingClusterPatch.after_renderer += dupRenlist.Count;
            EntryPoint.LogIt($"Cluster Renderer Count: {beforeCount} > {dupRenlist.Count}");
            return dupRenlist.ToArray();
        }
    }
}
