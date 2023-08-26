using CullingSystem;
using FluffyUnderware.Curvy.ThirdParty.LibTessDotNet;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppSystem;
using Il2CppInterop.Runtime;
using System.Collections;
using String = Il2CppSystem.String;

namespace LowSpecGaming.ResolutionPatch
{
    internal class Culling : MonoBehaviour
    {
        float accumTime = 0.0f;
        C_Cullable[] cullables;
        Vector3[] points;
        public static Vector3 playerPos;
        List<int> range;
        int slice;
        int part;
        float interval;
        int intervalMil;
        int count;
        private void Awake()
        {
            this.enabled = false;
            GTFO.API.LevelAPI.OnBuildDone += StartThis;
        }

        private void StartThis() {
            this.enabled = true;
            C_CullingManager.CullingEnabled = false;
            C_CullingManager.m_cullingEnabled = false;
        }

        public void GetRenderer() {
            var pointPositions = new List<Vector3>();
            var m_clusters = new Il2CppSystem.Collections.Generic.List<C_Cullable>();

            LG_Area[] objectsOfType1 = GameObject.FindObjectsOfType<LG_Area>();
            for (int index1 = 0; index1 < objectsOfType1.Length; ++index1)
            {
                LG_Area lgArea = objectsOfType1[index1];
                if (lgArea != null && lgArea.m_courseNode != null && lgArea.m_courseNode.m_cullNode != null)
                {
                    C_Node cullNode = lgArea.m_courseNode.m_cullNode;
                    for (int index2 = 0; index2 < cullNode.m_staticCullables.Count; ++index2)
                    {
                        var c = cullNode.m_staticCullables[index2];
                        m_clusters.Add(c);
                        pointPositions.Add(c.Bounds.center);

                    }
                }
            }
            cullables = m_clusters.ToArray();
            points = pointPositions.ToArray();
            EntryPoint.LogIt(m_clusters.Count);
        }

        public void SetDistance(int distance) { 
        }
        private void OnEnable()
        {
            GetRenderer();

            slice = 50;
            range = new List<int>();
            part = points.Length / slice;
            for (int i = 0; i < slice; i++)
            {
                range.Add(part * i);
            }
            range.Add(points.Length);
            EntryPoint.LogIt(range[range.Count-1]);
            interval = 2.5f / slice;
            intervalMil = (int)(interval * 1000);
            accumTime = 0f;
            count = 0;
        }
        private void Update()
        {
            accumTime += Time.deltaTime;
            Vector3 cameraPos = playerPos;
            if (accumTime > interval)
            {
                if (count > (range.Count - 2))
                    count = 0;
                DistanceCullStepped(count, cameraPos);
                count++;
            }
        }
        private void DistanceCullStepped(int a,Vector3 cameraPos)
        {
            Vector3 pos;
            for (int i = range[a]; i < range[a + 1]; i++)
            {
                pos = points[i];
                float x = (pos.x - cameraPos.x);
                float z = (pos.z - cameraPos.z);
                if (x * x + z * z < 3900)
                    cullables[i].Show();
                else
                    cullables[i].Hide();
            }
        }
        private void DistanceCull()
        {
            Debug.Log("We're Culling");
            Vector3 cameraPos = playerPos;
            Vector3 pos;
            for (int a = 0; a < range.Count - 1; a++)
            {
                for (int i = range[a]; i < range[a + 1]; i++)
                {
                    pos = points[i];
                    float x = (pos.x - cameraPos.x);
                    float z = (pos.z - cameraPos.z);
                    if (x * x + z * z < 4096)
                        cullables[i].Show();
                    else
                        cullables[i].Hide();
                }
                //await Task.Delay(intervalMil);
                //yield return new WaitForSeconds(interval);
            }
        }
    }
}
