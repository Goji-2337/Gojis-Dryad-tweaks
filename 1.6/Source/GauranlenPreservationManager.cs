using RimWorld;
using Verse;
using System.Collections.Generic;

namespace GojisDryadTweaks
{
    public static class GauranlenPreservationManager
    {
        private static Dictionary<Building_PlantGrower, Plant> treesToRespawn = new Dictionary<Building_PlantGrower, Plant>();
        private static List<Building_PlantGrower> keysWorkingList_Growers;
        private static List<Plant> valuesWorkingList_Plants;
        public static void StoreTree(Building_PlantGrower pot, Plant tree)
        {
            if (pot == null) return;
            if (!treesToRespawn.ContainsKey(pot))
            {
                treesToRespawn.Add(pot, tree);
            }
            else
            {
                treesToRespawn[pot] = tree;
            }
            tree.DeSpawn();
        }

        public static Plant GetTreeToRespawn(Building_PlantGrower pot)
        {
            if (pot == null) return null;
            treesToRespawn.TryGetValue(pot, out Plant tree);
            return tree;
        }

        public static void RemoveTreeFromRespawnList(Building_PlantGrower pot)
        {
            if (pot == null) return;
            if (treesToRespawn.ContainsKey(pot))
            {
                treesToRespawn.Remove(pot);
            }
        }

        public static bool IsPendingRespawn(Plant plant)
        {
            return treesToRespawn.ContainsValue(plant);
        }

        public static void ExposeData()
        {
            Scribe_Collections.Look(ref treesToRespawn, "treesToRespawn_Growers", LookMode.Reference, LookMode.Reference, ref keysWorkingList_Growers, ref valuesWorkingList_Plants);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (treesToRespawn == null)
                {
                    treesToRespawn = new Dictionary<Building_PlantGrower, Plant>();
                }
            }
        }
    }
}
