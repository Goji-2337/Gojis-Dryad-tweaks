using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace GojisDryadTweaks
{
    public class JobDriver_EnterCocoon : JobDriver_EnterDryadThingBase
    {
        protected override ThingDef DryadThingDef => GojiDefsOf.Goji_DryadCocoon;
    }
}
