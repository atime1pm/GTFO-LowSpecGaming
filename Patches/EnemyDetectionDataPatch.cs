using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Runtime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Enemies;
using HarmonyLib;

namespace LowSpecGaming.Patches
{
    [Harmony]
    [HarmonyPatch]
    internal static unsafe class EnemyDetectionDataPatch
    {
        private delegate void EnemyDataDelegate(IntPtr __instance);

        private static EnemyDataDelegate _Original;
        private static INativeDetour _Detour;

        public static void CreateDetour()
        {
            var description = new DetourDescription()
            {
                Type = typeof(EnemyDetection),
                MethodName = nameof(EnemyDetection.UpdateData),
                ReturnType = typeof(void),
                ArgTypes = new Type[] {},
                IsGeneric = false
            };

            Detour.TryCreate(description, Detour_EnemyData, out _Original, out _Detour);
        }
        private static void Detour_EnemyData(IntPtr __instance)
        {
            EnemyDetection.s_behaviourData = new EnemyDetection(__instance).m_ai.m_behaviourData;
            EnemyDetection.s_delta = Mathf.Clamp01(Clock.Time - EnemyDetection.s_behaviourData.LastDetectionUpdate);
            EnemyDetection.s_behaviourData.LastDetectionUpdate = Clock.Time;
            var agent = EnemyDetection.s_behaviourData.m_ai.Agent;
            EnemyDetection.s_pos = agent.Position;
            EnemyDetection.s_eyePos = agent.EyePosition;
            EnemyDetection.s_eyeDir = agent.Forward;
            EnemyDetection.s_bestTarget = null;
            EnemyDetection.s_bestTargetScore = -3.4028235E+38f;
        }
    }
}
