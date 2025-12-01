using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace GojisDryadTweaks
{
    public class JobDriver_EnterHealingPod : JobDriver_EnterDryadThingBase
    {
        protected override ThingDef DryadThingDef => ThingDefOf.DryadHealingPod;
        public override bool CanBeginNowWhileLyingDown()
        {
            return true;
        }

    }
}
