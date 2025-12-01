using HarmonyLib;
using Verse;
using RimWorld;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompTreeConnection), nameof(CompTreeConnection.GenerateNewDryad))]
    public static class CompTreeConnection_GenerateNewDryad_Patch
    {
        public static void Postfix(Pawn __result)
        {
            if (__result != null && CompDryadCocoon_Complete_Patch_StoreBond.bondedMaster != null)
            {
                CompDryadCocoon_Complete_Patch_StoreBond.bondedMaster.relations.AddDirectRelation(PawnRelationDefOf.Bond, __result);
            }
        }
    }
}
