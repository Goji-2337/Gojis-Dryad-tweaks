using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompPlantable), nameof(CompPlantable.CanPlantAt))]
    public static class CompPlantable_CanPlantAt_Patch
    {
        public static void Postfix(ref AcceptanceReport __result, IntVec3 cell, Map map, CompPlantable __instance)
        {
            if (!__result.Accepted)
            {
                List<Thing> thingsList = map.thingGrid.ThingsListAt(cell);
                for (int i = 0; i < thingsList.Count; i++)
                {
                    if (thingsList[i].def == GojiDefsOf.PlantPot_Bonsai)
                    {
                        ThingDef plantDef = __instance.Props.plantDefToSpawn;
                        AcceptanceReport plantResult = plantDef.CanEverPlantAt(cell, map, out _, canWipePlantsExceptTree: true);
                        if (plantResult.Accepted)
                        {
                            __result = AcceptanceReport.WasAccepted;
                            return;
                        }
                    }
                }
            }
        }
    }
}
