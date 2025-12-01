using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(PlantUtility), nameof(PlantUtility.AdjacentSowBlocker))]
    public static class PlantUtility_AdjacentSowBlocker_Patch
    {
        public static void Postfix(ref Thing __result, ThingDef plantDef, IntVec3 c, Map map)
        {
            if (__result != null && plantDef == ThingDefOf.Plant_TreeGauranlen)
            {
                List<Thing> thingsList = c.GetThingList(map);
                for (int i = 0; i < thingsList.Count; i++)
                {
                    if (thingsList[i].def == GojiDefsOf.PlantPot_Bonsai)
                    {
                        __result = null;
                        break;
                    }
                }
            }
        }
    }
}
