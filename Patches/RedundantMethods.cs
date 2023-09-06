using CullingSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowSpecGaming.Patches
{
    [HarmonyPatch]
    internal class RedundantMethods
    {
        //Doesn't really affect anything in the game
        //
        public static bool experimentalOn = true;
        [HarmonyPrefix][HarmonyPatch(typeof(CL_Light), nameof(CL_Light.UpdateData))]
        public static bool LightCull() => false;
        //I don't know the real effect of this so just gonna leave it here
        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClusteredRendering), nameof(ClusteredRendering.Update))]
        public static bool Cluster() => false;
        */
    }
}
