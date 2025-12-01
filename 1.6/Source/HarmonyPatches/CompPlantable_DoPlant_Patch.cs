using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompPlantable), nameof(CompPlantable.DoPlant))]
    public static class CompPlantable_DoPlant_Patch
    {
        public static void Prefix(Pawn planter, IntVec3 cell)
        {
            if (cell.GetFirstThing<Building_PlantGrower>(planter.Map) is Building_PlantGrower building_PlantGrower && building_PlantGrower.def == GojiDefsOf.PlantPot_Bonsai)
            {
                building_PlantGrower.plantDefToGrow = ThingDefOf.Plant_TreeGauranlen;
            }
        }
    }
}
