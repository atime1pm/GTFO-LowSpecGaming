using ChainedPuzzles;
using CullingSystem;
using Enemies;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowSpecGaming.Patches
{
    internal class PropagatePatch
    {

        public static IEnumerable<CodeInstruction> DisableTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            code.Clear();
            return code.AsEnumerable();
        }
    }
}
