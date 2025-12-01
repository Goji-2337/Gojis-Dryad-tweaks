using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(JobDriver_PruneGauranlenTre), nameof(JobDriver_PruneGauranlenTre.MakeNewToils))]
    public static class JobDriver_PruneGauranlenTre_MakeNewToils_Patch
    {
        public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_PruneGauranlenTre __instance)
        {
            var toils = values.ToList();
            var pawn = __instance.pawn;
            var lastToil = toils.LastOrDefault();
            if (lastToil != null)
            {
                lastToil.AddFinishAction(delegate
                {
                    if (pawn != null && pawn.genes != null && GojiDefsOf.VRE_GreenThumb != null && GojiDefsOf.VRE_GreenThumbHappy != null && pawn.genes.HasActiveGene(GojiDefsOf.VRE_GreenThumb))
                    {
                        pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(GojiDefsOf.VRE_GreenThumbHappy);
                    }
                });
            }
            return toils;
        }
    }
}
