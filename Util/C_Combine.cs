using LowSpecGaming.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Util
{
    public static class C_Combine
    {
        public static List<Renderer> CombineMeshes(List<Renderer> renderersToCombine)
        {
            if (renderersToCombine.Count < 2) return renderersToCombine;
            Material sharedMaterial;
            try 
            {
                sharedMaterial = renderersToCombine[0].sharedMaterial;

                foreach (Renderer rend in renderersToCombine)
                    if (rend.sharedMaterial != sharedMaterial)
                        return renderersToCombine;
            }
            catch 
            {
                sharedMaterial = renderersToCombine[0].material;

            }
            var ogName = renderersToCombine[0].gameObject.name;
            List<CombineInstance> combineInstances = new();


            foreach (var rend in renderersToCombine)
            {
                var mf = rend.GetComponent<MeshFilter>();

                if (mf == null) return renderersToCombine;

                CombineInstance combineInstance = new()
                {
                    mesh = mf.sharedMesh,
                    transform = rend.transform.localToWorldMatrix
                };
                combineInstances.Add(combineInstance);
                rend.enabled = false;
                rend.forceRenderingOff = true;
                GameObject.Destroy(mf);
            }

            GameObject combinedObject = new($"{ogName}_Combined");
            combinedObject.isStatic = true;

            C_CullingClusterPatch.CombinedGameObjects.Add(combinedObject);

            var combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            var combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

            Mesh combinedMesh = new();
            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
            combinedMeshFilter.mesh = combinedMesh;
            combinedMeshRenderer.sharedMaterial = sharedMaterial;

            combinedMeshRenderer.enabled = false;
            combinedMeshRenderer.shadowCastingMode = 0;
            combinedMeshRenderer.receiveShadows = false;
            combinedMeshRenderer.lightProbeUsage = 0;
            combinedMeshRenderer.reflectionProbeUsage = 0;
            
            return new List<Renderer> { combinedMeshRenderer as Renderer };
        }
    }
}
