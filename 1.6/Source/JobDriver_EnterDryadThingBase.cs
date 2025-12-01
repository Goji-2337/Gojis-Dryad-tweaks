using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace GojisDryadTweaks
{
    public abstract class JobDriver_EnterDryadThingBase : JobDriver
    {
        protected abstract ThingDef DryadThingDef { get; }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            if (TargetA.IsValid)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
            }
            yield return Toils_General.Wait(200).WithProgressBarToilDelay(TargetIndex.A);
            Toil enterThing = Toils_General.Do(delegate
            {
                Thing dryadThing = GenSpawn.Spawn(DryadThingDef, pawn.Position, pawn.Map);
                var podComp = dryadThing.TryGetComp<CompDryadHolder>();
                podComp.TryAcceptPawn(pawn);
                pawn.health.healthState = PawnHealthState.Mobile;
            });
            yield return enterThing;
        }
    }
}
