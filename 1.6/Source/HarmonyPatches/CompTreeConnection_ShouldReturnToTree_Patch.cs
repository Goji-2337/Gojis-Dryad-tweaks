using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompTreeConnection), "ShouldReturnToTree")]
    public static class CompTreeConnection_ShouldReturnToTree_Patch
    {
        public static bool ignore;
        public static void Postfix(CompTreeConnection __instance, ref bool __result, Pawn dryad)
        {
            if (ignore || __result is false) return;
            Pawn connectedPawn = __instance.ConnectedPawn;
            if (__result && connectedPawn?.Ideo?.HasPrecept(GojiDefsOf.Goji_DryadAutonomy_High) == true && connectedPawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, dryad))
            {
                __result = false;
            }
        }
    }
}
