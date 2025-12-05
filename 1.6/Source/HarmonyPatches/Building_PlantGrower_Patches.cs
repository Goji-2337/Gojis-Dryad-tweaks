using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch]
    public static class Building_PlantGrower_Patches
    {
        public static Building_PlantGrower toBeDespawned;
        
        [HarmonyPatch(typeof(Building_PlantGrower), nameof(Building_PlantGrower.DeSpawn))]
        [HarmonyPrefix]
        public static void DeSpawn_Prefix(Building_PlantGrower __instance)
        {
            if (__instance.def != GojiDefsOf.PlantPot_Bonsai) return;
            toBeDespawned = __instance;
            foreach (Plant plant in __instance.PlantsOnMe)
            {
                if (plant.def == ThingDefOf.Plant_TreeGauranlen)
                {
                    GauranlenPreservationManager.StoreTree(__instance, plant);
                }
            }
        }

        [HarmonyPatch(typeof(Building_PlantGrower), nameof(Building_PlantGrower.DeSpawn))]
        [HarmonyPostfix]
        public static void DeSpawn_Postfix(Building_PlantGrower __instance)
        {
            toBeDespawned = null;
        }

        [HarmonyPatch(typeof(Building), nameof(Building.Destroy))]
        [HarmonyPrefix]
        private static void Destroy_Prefix(Building __instance, DestroyMode mode)
        {
            if (__instance.def != GojiDefsOf.PlantPot_Bonsai || __instance is not Building_PlantGrower grower) return;
            foreach (Plant plant in grower.PlantsOnMe)
            {
                if (plant.def == ThingDefOf.Plant_TreeGauranlen)
                {
                    plant.Destroy(mode);
                }
            }
        }

        [HarmonyPatch(typeof(Building_PlantGrower), nameof(Building_PlantGrower.PlantsOnMe), MethodType.Getter)]
        [HarmonyPostfix]
        public static void PlantsOnMe_Postfix(Building_PlantGrower __instance, ref IEnumerable<Plant> __result)
        {
            if (__result == null) return;

            if (toBeDespawned == __instance)
            {
                var list = __result.ToList();
                int removedCount = list.RemoveAll(plant => plant.def == ThingDefOf.Plant_TreeGauranlen && GauranlenPreservationManager.IsPendingRespawn(plant));
                __result = list;
            }
        }

        [HarmonyPatch(typeof(Building_PlantGrower), nameof(Building_PlantGrower.ExposeData))]
        [HarmonyPostfix]
        public static void ExposeData_Postfix()
        {
            GauranlenPreservationManager.ExposeData();
        }

        [HarmonyPatch(typeof(Building_PlantGrower), nameof(Building_PlantGrower.SpawnSetup))]
        [HarmonyPostfix]
        public static void SpawnSetup_Postfix(Building_PlantGrower __instance, bool respawningAfterLoad)
        {
            Plant treeToRespawn = GauranlenPreservationManager.GetTreeToRespawn(__instance);
            if (treeToRespawn != null)
            {
                List<Thing> thingsInCell = __instance.Position.GetThingList(__instance.Map);
                for (int i = thingsInCell.Count - 1; i >= 0; i--)
                {
                    if (thingsInCell[i] is Plant existingPlant && existingPlant.def == ThingDefOf.Plant_TreeGauranlen && existingPlant != treeToRespawn)
                    {
                        existingPlant.Destroy(DestroyMode.Vanish);
                    }
                }
                GenSpawn.Spawn(treeToRespawn, __instance.Position, __instance.Map);
                GauranlenPreservationManager.RemoveTreeFromRespawnList(__instance);
            }
        }
    }
}
