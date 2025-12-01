using Verse;
using RimWorld;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GojisDryadTweaks
{
    public class RitualOutcomeEffectWorker_TurnDryadCocoon : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_TurnDryadCocoon() { }

        public RitualOutcomeEffectWorker_TurnDryadCocoon(RitualOutcomeEffectDef def) : base(def) { }

        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            base.Apply(progress, totalPresence, jobRitual);
            float quality = GetQuality(jobRitual, progress);
            RitualOutcomePossibility outcome = GetOutcome(quality, jobRitual);
            Pawn initiator = jobRitual.PawnWithRole("initiator");
            TargetInfo selectedTarget = jobRitual.selectedTarget;

            if (initiator == null || selectedTarget == null || !selectedTarget.HasThing)
            {
                Log.Error("TurnDryadCocoon ritual outcome worker could not find initiator or valid target tree.");
                return;
            }
            Thing targetTree = selectedTarget.Thing;
            if (targetTree == null)
            {
                Log.Error("TurnDryadCocoon ritual outcome worker target tree is null.");
                return;
            }

            IntVec3 spawnCell = IntVec3.Invalid;
            IntVec3 rootPos = targetTree.Position;
            Map map = targetTree.Map;

            Predicate<IntVec3> validator = (IntVec3 c) =>
                c.Standable(map) && GenConstruct.CanPlaceBlueprintAt(GojiDefsOf.Goji_HumanoidHealingCocoon, c, Rot4.South, map).Accepted;

            IntVec3 resultCell;
            if (!CellFinder.TryFindRandomCellNear(rootPos, map, 2, validator, out resultCell))
            {
                Log.Warning("TurnDryadCocoon ritual could not find valid spawn cell near tree. Trying fallback.");
                spawnCell = rootPos + IntVec3.North.RotatedBy(Rot4.Random);
                if (!spawnCell.Standable(map)) spawnCell = rootPos;
            }
            else
            {
                spawnCell = resultCell;
            }

            if (!spawnCell.IsValid)
            {
                Log.Error("Failed to determine a valid spawn cell for the cocoon.");
                return;
            }

            try
            {
                Thing cocoonThing = ThingMaker.MakeThing(GojiDefsOf.Goji_HumanoidHealingCocoon);
                GenSpawn.Spawn(cocoonThing, spawnCell, targetTree.Map, Rot4.South);
                CompHumanoidHealingCocoon cocoonComp = cocoonThing.TryGetComp<CompHumanoidHealingCocoon>();

                cocoonComp.AssignPawn(initiator, outcome.positivityIndex);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception spawning or assigning cocoon: {ex}");
            }
        }
    }
}
