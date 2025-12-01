using Verse;
using RimWorld;
using System.Collections.Generic;

namespace GojisDryadTweaks
{
    public class RitualRole_ConnectedTreeConnecter : RitualRole
    {
        public RitualRole_ConnectedTreeConnecter() { }

        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            reason = null;
            if (p == null || p.Dead || p.Downed)
            {
                if (!skipReason) reason = "MessageRitualPawnDowned".Translate(p?.LabelShortCap ?? "Pawn".Translate());
                return false;
            }
            if (!p.Awake())
            {
                if (!skipReason) reason = "MessageRitualPawnSleeping".Translate(p.LabelShortCap);
                return false;
            }
            if (!p.health.capacities.CapableOf(PawnCapacityDefOf.Moving))
            {
                if (!skipReason) reason = "MessageRitualPawnCannotMove".Translate(p.LabelShortCap);
                return false;
            }
            if (!selectedTarget.HasThing || !(selectedTarget.Thing is Plant))
            {
                if (!skipReason) reason = "RitualTargetNotTree".Translate();
                return false;
            }

            var treeComp = selectedTarget.Thing.TryGetComp<CompTreeConnection>();
            if (treeComp == null)
            {
                if (!skipReason) reason = "RitualTargetNotGauranlenTree".Translate();
                return false;
            }
            if (treeComp.ConnectedPawn != p)
            {
                if (!skipReason) reason = "Goji_MustBeConnectedPawn".Translate(p.Named("PAWN"), selectedTarget.Thing.Named("TREE"));
                return false;
            }
            return true;
        }

        public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn pawn = null, bool skipReason = false)
        {
            reason = null;
            return true;
        }
    }
}
