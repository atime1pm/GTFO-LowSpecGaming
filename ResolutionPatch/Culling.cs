using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.ResolutionPatch
{
    internal class Culling : MonoBehaviour
    {
        public ComputeShader distanceCalculationShader;
        Transform[] points = new Transform[0];
        public int[] indexFromBuffer;
        Il2CppSystem.Array irbArray;
        public int[] previousFromBuffer;
        Il2CppSystem.Array pfbArray;
        public int[] currentFromBuffer;
        Il2CppSystem.Array cfbArray;

        private ComputeBuffer currentBuffer;
        private ComputeBuffer outputBuffer;
        private ComputeBuffer prevBuffer;
        private ComputeBuffer distanceBuffer;
        private ComputeBuffer pointPositionsBuffer;

        public static UnityEngine.Object shader;
        float accumTime = 0.0f;
        private Vector3[] pointPositions = new Vector3[0];
        MeshRenderer[] rens;
        private void Awake()
        {

            EntryPoint.entry.Log.LogInfo(shader == null);

            this.enabled = false;

            //GTFO.API.LevelAPI.OnEnterLevel += OnEnable;
        }
        private void Load()
        {
            shader = Resources.Load("C:/Users/anhth/AppData/Roaming/r2modmanPlus-local/GTFO/profiles/Vox/BepInEx/plugins/LowSpecGaming/Sight/Shader/NotSharpen.shader");
        }
        [DllImport("__Internal")]
        private static extern IntPtr GetIl2CppArrayData(IntPtr il2cppArray);
        private void OnEnable()
        {
            pointPositions = new Vector3[points.Length];

            for (int i = 0; i < pointPositions.Length; i++)
            {
                pointPositions[i] = points[i].position;
            }
            rens = new MeshRenderer[points.Length];
            for (int i = 0; i < rens.Length; i++)
            {
                rens[i] = points[i].gameObject.GetComponent<MeshRenderer>();
            }


            pointPositionsBuffer = new ComputeBuffer(pointPositions.Length, sizeof(float) * 3);
            distanceBuffer = new ComputeBuffer(pointPositions.Length, sizeof(float));
            currentBuffer = new ComputeBuffer(pointPositions.Length + 1, sizeof(int));
            prevBuffer = new ComputeBuffer(pointPositions.Length + 1, sizeof(int));
            outputBuffer = new ComputeBuffer(pointPositions.Length + 1, sizeof(int));

            Il2CppSystem.Array ppArray = new Il2CppSystem.Array();
            pointPositions.TryCastTo(out ppArray);

            pointPositionsBuffer.SetData(ppArray);
            distanceCalculationShader.SetBuffer(0, "distanceBuffer", distanceBuffer);
            distanceCalculationShader.SetBuffer(0, "pointPositions", pointPositionsBuffer);
            distanceCalculationShader.SetInt("arraysize", pointPositions.Length);
            distanceCalculationShader.SetBuffer(0, "currentBuffer", currentBuffer);
            distanceCalculationShader.SetBuffer(0, "outputBuffer", outputBuffer);
            distanceCalculationShader.SetBuffer(0, "prevBuffer", prevBuffer);


            indexFromBuffer = new int[currentBuffer.count];
            irbArray = new Il2CppSystem.Array();
            indexFromBuffer.TryCastTo(out irbArray);

            previousFromBuffer = new int[currentBuffer.count];
            pfbArray = new Il2CppSystem.Array();
            previousFromBuffer.TryCastTo(out pfbArray);

            currentFromBuffer = new int[currentBuffer.count];
            cfbArray = new Il2CppSystem.Array();
            currentFromBuffer.TryCastTo(out cfbArray);
        }
        private void Update()
        {
            distanceCalculationShader.SetVector("floatingPoint", Camera.main.transform.position);

            outputBuffer.GetData(irbArray);
            currentBuffer.GetData(pfbArray);
            Marshal.Copy(irbArray.Pointer, indexFromBuffer, 0, indexFromBuffer.Length);
            distanceCalculationShader.Dispatch(0, 1, 1, 1);
            for (int i = 0; i < indexFromBuffer[irbArray.GetLength(0) - 1]; i++)
            {
                if (indexFromBuffer[i] > 0)
                    rens[indexFromBuffer[i]].forceRenderingOff = false;
                if (indexFromBuffer[i] < 0)
                    rens[-indexFromBuffer[i]].forceRenderingOff = true;
            }

            prevBuffer.SetData(pfbArray);

            distanceCalculationShader.SetBuffer(0, "prevBuffer", prevBuffer);
        }
        private void OnDestroy()
        {
            
        }
    }
}
