using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;
using Verse.AI;

namespace GojisDryadTweaks
{

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public static class Pawn_Kill_Patch
    {
        public static bool Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (__instance?.RaceProps == null || !__instance.RaceProps.Dryad)
            {
                return true;
            }
            Thing tree = __instance.connections?.ConnectedThings.FirstOrDefault(t => t.def == ThingDefOf.Plant_TreeGauranlen);
            CompTreeConnection treeComp = tree?.TryGetComp<CompTreeConnection>();
            Pawn pruner = treeComp?.ConnectedPawn;
            if (pruner?.Ideo != null && pruner.Ideo.HasPrecept(GojiDefsOf.Goji_DryadLifespan_Immortal))
            {
                if (__instance.Spawned && __instance.jobs?.curJob?.def != GojiDefsOf.Goji_EnterHealingPod)
                {
                    Job job = JobMaker.MakeJob(GojiDefsOf.Goji_EnterHealingPod, __instance.Position);
                    __instance.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }
                return false;
            }
            return true;
        }
    }
}
