using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;
using System;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompDryadHealingPod), nameof(CompDryadHealingPod.Complete))]
    public static class CompDryadHealingPod_Complete_Patch
    {
        public static void Prefix(CompDryadHealingPod __instance)
        {
            Pawn dryad = __instance.innerContainer?.FirstOrDefault() as Pawn;
            if (dryad == null || dryad.RaceProps.animalType != AnimalType.Dryad || dryad.connections == null)
            {
                return;
            }

            Thing connectedTree = dryad.connections.ConnectedThings.FirstOrDefault();
            if (connectedTree == null)
            {
                return;
            }

            CompTreeConnection treeComp = connectedTree.TryGetComp<CompTreeConnection>();
            if (treeComp == null)
            {
                return;
            }

            Pawn connectedPawn = treeComp.ConnectedPawn;
            if (connectedPawn?.Ideo?.HasPrecept(GojiDefsOf.Goji_DryadLifespan_Immortal) == true)
            {
                long ageReductionTicks = GenDate.TicksPerYear;
                long currentAgeTicks = dryad.ageTracker.AgeBiologicalTicks;
                long newAgeTicks = Math.Max(0L, currentAgeTicks - ageReductionTicks);

                if (newAgeTicks < currentAgeTicks)
                {
                    dryad.ageTracker.AgeBiologicalTicks = newAgeTicks;
                }
            }
        }
    }
}
