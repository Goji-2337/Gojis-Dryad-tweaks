using Verse;
using RimWorld;
using System.Collections.Generic;

namespace GojisDryadTweaks
{
    public class RitualObligationTargetWorker_ConnectedGauranlenTree : RitualObligationTargetFilter
    {
        public RitualObligationTargetWorker_ConnectedGauranlenTree() { }

        public RitualObligationTargetWorker_ConnectedGauranlenTree(RitualObligationTargetFilterDef def) : base(def) { }

        public override IEnumerable<TargetInfo> GetTargets(RitualObligation obligation, Map map)
        {
            List<Thing> trees = map.listerThings.ThingsOfDef(ThingDefOf.Plant_TreeGauranlen);
            for (int i = 0; i < trees.Count; i++)
            {
                CompTreeConnection compTreeConnection = trees[i].TryGetComp<CompTreeConnection>();
                if (compTreeConnection != null && compTreeConnection.Connected)
                {
                    yield return trees[i];
                }
            }
        }

        public override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation)
        {
            Thing thing = target.Thing;
            if (thing == null || thing.def != ThingDefOf.Plant_TreeGauranlen)
            {
                return false;
            }

            CompTreeConnection compTreeConnection = thing.TryGetComp<CompTreeConnection>();
            if (compTreeConnection == null)
            {
                return false;
            }
            if (!compTreeConnection.Connected)
            {
                return "Goji_RitualTargetMustBeConnectedGauranlenTree".Translate();
            }

            return true;
        }

        public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
        {
            yield return "Goji_RitualTargetConnectedGauranlenTreeInfo".Translate();
        }
    }
}
