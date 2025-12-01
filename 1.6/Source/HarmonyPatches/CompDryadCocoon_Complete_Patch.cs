using HarmonyLib;
using Verse;
using RimWorld;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompDryadCocoon), nameof(CompDryadCocoon.Complete))]
    public static class CompDryadCocoon_Complete_Patch_StoreBond
    {
        public static Pawn bondedMaster;
        public static void Prefix(CompDryadCocoon __instance)
        {
            var treeComp = __instance.TreeComp;
            if (treeComp != null && treeComp.ConnectedPawn != null && __instance.innerContainer.FirstOrDefault() is Pawn dryad && treeComp.ConnectedPawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, dryad))
            {
                bondedMaster = treeComp.ConnectedPawn;
            }
        }

        public static void Postfix()
        {
            bondedMaster = null;
        }
    }
}
