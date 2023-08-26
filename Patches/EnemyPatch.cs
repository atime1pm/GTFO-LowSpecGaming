using Agents;
using Enemies;
using HarmonyLib;
using ShaderValueAnimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (EntryPoint.enemyBehaviourCulling.Value == Structs.EnemyBehaviourCulling.Full)
            { return true; }
            ShaderValueAnimator.ManagerSetup();
            __instance.m_nodeUpdates = new EnemyUpdateManager.UpdateNodeGroup();
            __instance.m_nodeUpdates.Setup(1);
            __instance.m_locomotionUpdatesClose = new EnemyUpdateManager.UpdateLocomotionGroup();
            __instance.m_locomotionUpdatesClose.Setup(12);//20
            __instance.m_locomotionUpdatesNear = new EnemyUpdateManager.UpdateLocomotionGroup();
            __instance.m_locomotionUpdatesNear.Setup(3, __instance.m_locomotionUpdatesClose);//10
            __instance.m_locomotionUpdatesFar = new EnemyUpdateManager.UpdateLocomotionGroup();
            __instance.m_locomotionUpdatesFar.Setup(1, __instance.m_locomotionUpdatesNear);//5
            __instance.m_fixedLocomotionUpdatesClose = new EnemyUpdateManager.FixedUpdateLocomotionGroup();
            __instance.m_fixedLocomotionUpdatesClose.Setup(3);//5
            __instance.m_fixedLocomotionUpdatesNear = new EnemyUpdateManager.FixedUpdateLocomotionGroup();
            __instance.m_fixedLocomotionUpdatesNear.Setup(1, __instance.m_fixedLocomotionUpdatesClose);//2
            __instance.m_fixedLocomotionUpdatesFar = new EnemyUpdateManager.FixedUpdateLocomotionGroup();
            __instance.m_fixedLocomotionUpdatesFar.Setup(1, __instance.m_fixedLocomotionUpdatesNear);//1
            __instance.m_behaviourUpdatesClose = new EnemyUpdateManager.UpdateBehaviourGroup();
            __instance.m_behaviourUpdatesClose.Setup(1);//5
            __instance.m_behaviourUpdatesNear = new EnemyUpdateManager.UpdateBehaviourGroup();
            __instance.m_behaviourUpdatesNear.Setup(1, __instance.m_behaviourUpdatesClose);//2
            __instance.m_behaviourUpdatesFar = new EnemyUpdateManager.UpdateBehaviourGroup();
            __instance.m_behaviourUpdatesFar.Setup(1, __instance.m_behaviourUpdatesNear);//1
            __instance.m_detectionUpdatesClose = new EnemyUpdateManager.UpdateDetectionGroup();
            __instance.m_detectionUpdatesClose.Setup(10);//10
            __instance.m_detectionUpdatesNear = new EnemyUpdateManager.UpdateDetectionGroup();
            __instance.m_detectionUpdatesNear.Setup(3, __instance.m_detectionUpdatesNear);//4
            __instance.m_detectionUpdatesFar = new EnemyUpdateManager.UpdateDetectionGroup();
            __instance.m_detectionUpdatesFar.Setup(1, __instance.m_detectionUpdatesFar);//1
            __instance.m_networkUpdatesClose = new EnemyUpdateManager.UpdateNetworkGroup();
            __instance.m_networkUpdatesClose.Setup(12);//12
            __instance.m_networkUpdatesNear = new EnemyUpdateManager.UpdateNetworkGroup();
            __instance.m_networkUpdatesNear.Setup(3, __instance.m_networkUpdatesClose);//3
            __instance.m_networkUpdatesFar = new EnemyUpdateManager.UpdateNetworkGroup();
            __instance.m_networkUpdatesFar.Setup(1, __instance.m_networkUpdatesNear);//1
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ES_Hitreact), nameof(ES_Hitreact.ActivateState))]
        public static void Killit(ES_Hitreact __instance, ref ES_HitreactType hitreactType)
        {
            if (hitreactType == ES_HitreactType.ToDeath)
            {
                //I hate this yonked this from Flow's Github... Im sorry
                //Trying to make enemies die faster....
                //FIXME: make enemies despawn earlier
                var agent = __instance.m_enemyAgent;
                var updateMode = __instance.m_enemyAgent.UpdateMode;

                if (updateMode == NodeUpdateMode.None)
                {
                    EnemyUpdateManager.Current.Register(agent, NodeUpdateMode.Near);
                }
            }

        }
    }
}
