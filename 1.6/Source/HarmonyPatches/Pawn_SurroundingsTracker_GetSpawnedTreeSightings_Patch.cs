using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(Pawn_SurroundingsTracker), "GetSpawnedTreeSightings")]
    public static class Pawn_SurroundingsTracker_GetSpawnedTreeSightings_Patch
    {
        public static void Postfix(Pawn_SurroundingsTracker __instance)
        {
            foreach (TreeSighting sighting in __instance.superTreeSightings.ToList())
            {
                if (sighting.Tree != null && sighting.Tree.IsGauranlenTreeInBonsaiPot())
                {
                    __instance.superTreeSightings.Remove(sighting);
                    __instance.miniTreeSightings.Add(sighting);
                }
            }
        }
    }
}
