using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(InfestationCellFinder), "GetScoreAt")]
    public static class InfestationCellFinder_GetScoreAt_Patch
    {
        private static List<ThingDef> superTreeDefsCache;
        private const float SuperTreeProtectionRadius = 10f;
        public static void Postfix(ref float __result, IntVec3 cell, Map map)
        {
            if (__result <= 0f)
            {
                return;
            }
            if (superTreeDefsCache.NullOrEmpty())
            {
                superTreeDefsCache = new List<ThingDef>();
                foreach (ThingDef td in DefDatabase<ThingDef>.AllDefs)
                {
                    if (td.plant != null && td.plant.treeCategory == TreeCategory.Super)
                    {
                        superTreeDefsCache.Add(td);
                    }
                }
            }

            foreach (ThingDef superTreeDef in superTreeDefsCache)
            {
                List<Thing> superTrees = map.listerThings.ThingsOfDef(superTreeDef);
                foreach (Thing tree in superTrees)
                {
                    if (cell.InHorDistOf(tree.Position, SuperTreeProtectionRadius))
                    {
                        __result = 0f;
                        return;
                    }
                }
            }
        }
    }
}
