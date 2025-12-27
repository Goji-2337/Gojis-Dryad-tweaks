using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
    public static class DefGenerator_GenerateImpliedDefs_PreResolve_Patch
    {
        public static void Postfix()
        {
            var dryads = DefDatabase<ThingDef>.AllDefsListForReading.
            Where(x => x.race?.thinkTreeMain != null && x.race.Dryad).ToList();
            foreach (var dryad in dryads)
            {
                dryad.race.nuzzleMtbHours = 12;
            }
            var dryadThinkTreeDefs = dryads.Select(x => x.race.thinkTreeMain).Distinct().ToList();
            foreach (var thinkTreeDef in dryadThinkTreeDefs)
            {
                if (thinkTreeDef.thinkRoot.subNodes == null)
                {
                    thinkTreeDef.thinkRoot.subNodes = new List<ThinkNode>();
                }
                var lordDutyNode = thinkTreeDef.thinkRoot.subNodes
                    .FirstOrDefault(n => n is ThinkNode_Subtree subtree && subtree.treeDef != null && subtree.treeDef.defName == "LordDuty");
                if (lordDutyNode != null)
                {
                    var index = thinkTreeDef.thinkRoot.subNodes.IndexOf(lordDutyNode);
                    thinkTreeDef.thinkRoot.subNodes.Insert(index + 1, new JobGiver_Nuzzle());
                }
            }
        }
    }
}
