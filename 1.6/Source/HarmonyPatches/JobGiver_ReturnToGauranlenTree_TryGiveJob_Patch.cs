using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;
using Verse.AI;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(JobGiver_ReturnToGauranlenTree), "TryGiveJob")]
    public static class JobGiver_ReturnToGauranlenTree_TryGiveJob_Patch
    {
        public static bool Prefix(Pawn pawn, ref Job __result)
        {
            CompTreeConnection_ShouldReturnToTree_Patch.ignore = true;
            Thing connectedTree = pawn.connections?.ConnectedThings.FirstOrDefault();
            CompTreeConnection treeComp = connectedTree?.TryGetComp<CompTreeConnection>();
            if (treeComp == null || !treeComp.ShouldReturnToTree(pawn))
            {
                CompTreeConnection_ShouldReturnToTree_Patch.ignore = false;
                return true;
            }
            Pawn connected = treeComp.ConnectedPawn;
            if (connected != null && connected.Ideo != null && connected.Ideo.HasPrecept(GojiDefsOf.Goji_DryadAutonomy_High) && connected.relations.DirectRelationExists(PawnRelationDefOf.Bond, pawn))
            {
                CompTreeConnection_ShouldReturnToTree_Patch.ignore = false;
                var position = RCellFinder.BestOrderedGotoDestNear(pawn.Position, pawn, (IntVec3 x) => x.GetThingList(pawn.Map).Any() is false);
                __result = JobMaker.MakeJob(GojiDefsOf.Goji_EnterCocoon, position);
                return false;
            }
            CompTreeConnection_ShouldReturnToTree_Patch.ignore = false;
            return true;
        }
    }
}
