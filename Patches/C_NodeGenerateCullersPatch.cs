using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CullingSystem;
using SpatialPartitionUtil;
using UnityEngine.Rendering;
using UnityEngine;
using LowSpecGaming.Util;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]

    internal class C_NodeGenerateCullersPatch
    {
        public static List<C_CullingCluster> clusters = new List<C_CullingCluster>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(C_NodeGenerateCullers), nameof(C_NodeGenerateCullers.Build))]
        public static bool Build_Patch2(C_NodeGenerateCullers __instance, ref bool __result) 
        {
            switch (__instance.m_state)
            {
                case C_NodeGenerateCullers.State.Setup:
                    __instance.m_cNode.SpatialPartitions = (SpatialPartitions_BigObjectsFilter<Renderer, SpatialPartitionedRenderers>)null;
                    __instance.m_clusterStep = 0;
                    __instance.m_clusterIDOffset = 0;
                    __instance.m_clusterMax = __instance.m_clusterinJob.m_clusterBounds.Length;
                    __instance.m_singleCullerStep = 0;
                    __instance.m_singleCullerMax = __instance.m_clusterinJob.m_clusterIJob.m_singleCullerClusters_Bounds.Count;
                    __instance.m_state = C_NodeGenerateCullers.State.GenerateJobClusters;
                    break;
                case C_NodeGenerateCullers.State.GenerateJobClusters:
                    if (__instance.m_clusterStep < __instance.m_clusterMax)
                    {
                        int clusterIdCount = __instance.m_clusterinJob.m_clusterIDCounts[__instance.m_clusterStep];
                        int clusterIdOffset = __instance.m_clusterIDOffset;
                        int num = __instance.m_clusterIDOffset + clusterIdCount;
                        for (int index = clusterIdOffset; index < num; ++index)
                        {
                            __instance.m_renderer = __instance.m_cNode.Renderers[(int)__instance.m_clusterinJob.m_clusterIDs[index]];
                            switch (__instance.m_renderer.shadowCastingMode)
                            {
                                case ShadowCastingMode.Off:
                                    __instance.m_renderers.Add(__instance.m_renderer);
                                    break;
                                case ShadowCastingMode.On:
                                    __instance.m_renderers.Add(__instance.m_renderer);
                                    __instance.m_shadowRenderers.Add(__instance.m_renderer);
                                    break;
                                case ShadowCastingMode.ShadowsOnly:
                                    __instance.m_renderer.enabled = typeof(SkinnedMeshRenderer) == ((object)__instance.m_renderer).GetType();
                                    __instance.m_shadowRenderers.Add(__instance.m_renderer);
                                    break;
                                default:
                                    EntryPoint.LogIt("Error while generating CULLING CLUSTER");
                                    break;
                            }
                        }
                        Renderer[] rens = __instance.m_renderers.ToArray();
                        rens = ClusterRestruct.Restruct(rens);
                        Renderer[] shadow_rens = __instance.m_shadowRenderers.ToArray();
                        __instance.m_clusters.Add(new C_CullingCluster(__instance.m_clusterinJob.m_clusterBounds[__instance.m_clusterStep], rens, shadow_rens));
                        __instance.m_renderers.Clear();
                        __instance.m_shadowRenderers.Clear();
                        __instance.m_clusterIDOffset += clusterIdCount;
                        ++__instance.m_clusterStep;
                        break;
                    }
                    __instance.m_state = C_NodeGenerateCullers.State.GenerateSingleCullerClusters;
                    break;
                case C_NodeGenerateCullers.State.GenerateSingleCullerClusters:
                    if (__instance.m_singleCullerStep < __instance.m_singleCullerMax)
                    {
                        __instance.m_renderer = __instance.m_cNode.Renderers[__instance.m_clusterinJob.m_clusterIJob.m_singleCullerClusters_refID[__instance.m_singleCullerStep]];
                        switch (__instance.m_renderer.shadowCastingMode)
                        {
                            case ShadowCastingMode.Off:
                                __instance.m_clusters.Add(new C_CullingCluster(__instance.m_clusterinJob.m_clusterIJob.m_singleCullerClusters_Bounds[__instance.m_singleCullerStep], new Renderer[1]
                                {
                  __instance.m_renderer
                                }, new Renderer[0]));
                                break;
                            case ShadowCastingMode.On:
                                __instance.m_clusters.Add(
                                    new C_CullingCluster
                                        (
                                            __instance.m_clusterinJob.m_clusterIJob.m_singleCullerClusters_Bounds[__instance.m_singleCullerStep], 
                                            new Renderer[1]{__instance.m_renderer},
                                            new Renderer[1] { __instance.m_renderer }
                                        )
                                    );
                                break;
                            case ShadowCastingMode.ShadowsOnly:
                                __instance.m_renderer.enabled = typeof(SkinnedMeshRenderer) == ((object)__instance.m_renderer).GetType();
                                __instance.m_clusters.Add(new C_CullingCluster(__instance.m_clusterinJob.m_clusterIJob.m_singleCullerClusters_Bounds[__instance.m_singleCullerStep], new Renderer[0], new Renderer[1]
                                {
                  __instance.m_renderer
                                }));
                                break;
                        }
                        ++__instance.m_singleCullerStep;
                        break;
                    }
                    foreach (var c in __instance.m_clusters) 
                    {
                        var cl = c.TryCast<C_Cullable>();
                        if (cl != null)
                        {
                            __instance.m_cNode.m_staticCullables.Add(cl);
                        }
                    }   
                    __instance.m_cNode.Renderers.Clear();
                    __instance.m_cNode.SpecialRenderableObjects.Clear();
                    if (C_CullingManager.CullingEnabled)
                    {
                        __instance.m_cNode.Hide();
                        for (int index = 0; index < __instance.m_cNode.m_staticCullables.Count; ++index)
                        {
                            __instance.m_cNode.m_staticCullables[index].IsShown = true;
                            __instance.m_cNode.m_staticCullables[index].Hide();
                        }
                    }
                    __result = true;
                    return false;
            }
            __result = false;
            return false;
        }
    }
}
