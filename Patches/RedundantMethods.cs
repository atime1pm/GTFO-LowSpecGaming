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
        [HarmonyPrefix][HarmonyPatch(typeof(CL_Light), nameof(CL_Light.UpdateData))]
        public static bool LightCull() => false;
    }
}
