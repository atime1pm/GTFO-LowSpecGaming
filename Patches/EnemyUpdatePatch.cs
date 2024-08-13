using ChainedPuzzles;
using Enemies;
using Gear;
using HarmonyLib;
using ShaderValueAnimation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LowSpecGaming.Patches
{
    [Harmony]
    [HarmonyPatch]
    internal class EnemyUpdatePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyUpdateManager), nameof(EnemyUpdateManager.GamestateUpdateFixed))]
        public static bool EnemyUpdateManagerPatch2(EnemyUpdateManager __instance)
        {
            ShaderValueAnimator.ManagerUpdate();
            __instance.m_nodeUpdates.Update();
            __instance.m_locomotionUpdatesClose.Update();
            __instance.m_locomotionUpdatesNear.Update();
            __instance.m_locomotionUpdatesFar.Update();
            __instance.m_fixedLocomotionUpdatesClose.Update();
            __instance.m_fixedLocomotionUpdatesNear.Update();
            __instance.m_fixedLocomotionUpdatesFar.Update();
            return false;
        }
    }
}
