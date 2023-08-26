using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Mathematics;
using FluffyUnderware.DevTools.Extensions;
using AssetShards;
using GameData;
using LowSpecGaming.ResolutionPatch;
using System.Diagnostics.CodeAnalysis;
using LowSpecGaming.Structs;
using LowSpecGaming.Patches;

namespace LowSpecGaming
{
    internal class LowSpecGaming : MonoBehaviour
    {
        public static Vector3 playerPos;
        public static AssetBundle assetBundle;
        public static UnityEngine.Object comp;
        public void Start() {
            GTFO.API.LevelAPI.OnEnterLevel += GetTheNav;
            if (EntryPoint.gameEnvironment.Value == GameEnvironment.Reduced)
            { 
                GTFO.API.LevelAPI.OnEnterLevel += HateTheGameFeel;
            }
            Detour_DrawMeshInstancedIndirect.CreateDetour();
            EntryPoint.GetTheSettings();
            Invoke("ApplySettings", 7f);
            comp = LoadShader("Assets/OcclusionCulling.compute");
        }
        public static void ApplySettings() {
            int value = (int)EntryPoint.textureSize.Value;
            EntryPoint.LogIt("Apply Settings");
            StartUpSettings.gameLoaded = true;
            ResolutionPatch.ResolutionPatch.canvasScale = CellSettingsManager.SettingsData.Video.Resolution.Value.y / 1080f;
            StartUpSettings.PotatoTexture(ref value);
        }
        private static void GetTheNav() {
        }
        private static void HateTheGameFeel() {
            PreLitVolume.Current.gameObject.GetComponent<AmbientParticles>().enabled = false;
            PreLitVolume.Current.m_fogDistance = 45;
            PreLitVolume.Current.FogPostBlur= 1;
            PreLitVolume.Current.FogShadowSamples = 0;
            PreLitVolume.Current.IndirectBlurSamples = 0;
            PreLitVolume.Current.IndirectDownsampling= 0;
            QualitySettings.shadowDistance = 15;
            QualitySettings.softParticles = false;
        }
        public static Mesh CreateCubeMesh(Vector3 size)
        {
            Mesh mesh = new Mesh();

            Vector3 halfSize = size * 0.5f;

            // Define vertices of the cube
            Vector3[] vertices = new Vector3[]
            {
            new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, -halfSize.y, halfSize.z),

            new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, halfSize.z)
            };

            // Define triangles of the cube
            int[] triangles = new int[]
            {
            0, 1, 2, 0, 2, 3, // Bottom face
            4, 6, 5, 4, 7, 6, // Top face
            0, 4, 1, 1, 4, 5, // Front face
            1, 5, 2, 2, 5, 6, // Right face
            2, 6, 3, 3, 6, 7, // Back face
            3, 7, 0, 0, 7, 4  // Left face
            };

            // Set vertices and triangles of the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            // Recalculate normals for proper lighting
            mesh.RecalculateNormals();

            return mesh;
        }
        public static void ClusterRenderingOff() {
            ClusteredRendering.Current.enabled = false;
        }
        public static UnityEngine.Object LoadShader(string path)
        {
            comp = GTFO.API.AssetAPI.GetLoadedAsset(path);
            return GTFO.API.AssetAPI.GetLoadedAsset(path);
        }
        IEnumerator LoadAsset(string assetBundleName, string objectNameToLoad)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles");
            filePath = System.IO.Path.Combine(filePath, assetBundleName);

            //Load "animals" AssetBundle
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);
            yield return assetBundleCreateRequest;

            AssetBundle asseBundle = assetBundleCreateRequest.assetBundle;

            //Load the "dog" Asset (Use Texture2D since it's a Texture. Use GameObject if prefab)
            AssetBundleRequest asset = asseBundle.LoadAssetAsync<Texture2D>(objectNameToLoad);
            yield return asset;

             

            //Retrieve the object (Use Texture2D since it's a Texture. Use GameObject if prefab)
            //Texture2D loadedAsset = asset.asset as Texture2D;
        }
    }
}
