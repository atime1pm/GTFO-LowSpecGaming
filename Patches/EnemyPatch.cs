﻿using Agents;
using Enemies;
using HarmonyLib;
using ShaderValueAnimation;
using System;
using System.Collections.Generic;
using System.Linq;
using static Enemies.EnemyUpdateManager;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class EnemyPatch
    { 

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyUpdateManager), nameof(EnemyUpdateManager.Setup))]
        public static bool EnemyCulling(EnemyUpdateManager __instance)
        {
            if (EntryPoint.enemyBehaviourCulling.Value == EnemyBehaviourCulling.Full) return true;

            ShaderValueAnimator.ManagerSetup();
            __instance.m_nodeUpdates = new();
            __instance.m_nodeUpdates.Setup(1);

            //This number means how many enemies can update per frame
            //Not how many frames the enemies can update
            //
            __instance.m_locomotionUpdatesClose = new();
            __instance.m_locomotionUpdatesClose.Setup(18);//20
            __instance.m_locomotionUpdatesNear = new();
            __instance.m_locomotionUpdatesNear.Setup(9, __instance.m_locomotionUpdatesClose);//10
            __instance.m_locomotionUpdatesFar = new();
            __instance.m_locomotionUpdatesFar.Setup(4, __instance.m_locomotionUpdatesNear);//5 -- lowering this too much causes problem

            //Should be the enemy fixed updates
            //60 per sec?
            __instance.m_fixedLocomotionUpdatesClose = new();
            __instance.m_fixedLocomotionUpdatesClose.Setup(3);//5
            __instance.m_fixedLocomotionUpdatesNear = new();
            __instance.m_fixedLocomotionUpdatesNear.Setup(2, __instance.m_fixedLocomotionUpdatesClose);//2
            __instance.m_fixedLocomotionUpdatesFar = new();
            __instance.m_fixedLocomotionUpdatesFar.Setup(1, __instance.m_fixedLocomotionUpdatesNear);//1


            //How the enemy changes state
            //
            __instance.m_behaviourUpdatesClose = new();
            __instance.m_behaviourUpdatesClose.Setup(2);//5
            __instance.m_behaviourUpdatesNear = new ();
            __instance.m_behaviourUpdatesNear.Setup(1, __instance.m_behaviourUpdatesClose);//2
            __instance.m_behaviourUpdatesFar = new ();
            __instance.m_behaviourUpdatesFar.Setup(1, __instance.m_behaviourUpdatesNear);//1


            //Turning this down too much makes the enemy detects slower
            //
            __instance.m_detectionUpdatesClose = new();
            __instance.m_detectionUpdatesClose.Setup(10);//10
            __instance.m_detectionUpdatesNear = new();
            __instance.m_detectionUpdatesNear.Setup(3, __instance.m_detectionUpdatesNear);//4
            __instance.m_detectionUpdatesFar = new();
            __instance.m_detectionUpdatesFar.Setup(1, __instance.m_detectionUpdatesFar);//1
            //Dont change this, causes some slight Desync
            __instance.m_networkUpdatesClose = new();
            __instance.m_networkUpdatesClose.Setup(12);//12
            __instance.m_networkUpdatesNear = new ();
            __instance.m_networkUpdatesNear.Setup(3, __instance.m_networkUpdatesClose);//3
            __instance.m_networkUpdatesFar = new();
            __instance.m_networkUpdatesFar.Setup(1, __instance.m_networkUpdatesNear);//1
            return false;
        }
    }
}
