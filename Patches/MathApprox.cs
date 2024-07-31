using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LowSpecGaming.Patches
{
    public static class MathApprox
    {
        public static readonly int TableSize = 362; // Number of entries in the table
        public static readonly float[] SinTable = new float[TableSize];
        public static readonly float[] CosTable = new float[TableSize];
        public static void Init()
        {
            for (int i = 0; i < TableSize; i++)
            {
                float angleInRadians = Mathf.Deg2Rad * i;
                SinTable[i] = Mathf.Sin(angleInRadians);
                CosTable[i] = Mathf.Cos(angleInRadians);
            }
        }
        public static float Sin(float angle)
        {
            if (angle < 0) angle += 360;
            return SinTable[(Mathf.RoundToInt(angle))];
        }
        public static float SSin(float angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;
            return SinTable[(Mathf.RoundToInt(angle))];
        }

        // Method to get cosine value from the table
        public static float Cos(float angle)
        {
            if (angle < 0) angle += 360;
            return CosTable[(Mathf.RoundToInt(angle))];
        }
        public static float SCos(float angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;
            return CosTable[(Mathf.RoundToInt(angle))];
        }
    }
}
