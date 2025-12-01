using Verse;
using RimWorld;
using System.Linq;

namespace GojisDryadTweaks
{
    public class RitualRoleColonistHealable : RitualRoleColonist
    {
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            if (!base.AppliesToPawn(p, out reason, selectedTarget, ritual, assignments, precept, skipReason: true))
            {
                if (!skipReason)
                {
                    reason = "Goji_MustBeColonist".Translate();
                }
                return false;
            }

            if (!p.RaceProps.Humanlike)
            {
                if (!skipReason)
                {
                    reason = "Goji_MustBeHumanlike".Translate();
                }
                return false;
            }

            bool needsHealing = false;
            if (p.health?.hediffSet != null)
            {
                needsHealing = p.health.hediffSet.hediffs.Any(h =>
                    h is Hediff_Injury ||
                    (h.def.isBad && h.def.makesSickThought) ||
                    h.def.HasModExtension<DiseaseMarker>()
                );
            }

            if (!needsHealing)
            {
                if (!skipReason)
                {
                    reason = "Goji_MustHaveInjuryOrIllnessForCocoon".Translate(p.Named("PAWN"));
                }
                return false;
            }

            if (!skipReason)
            {
                reason = null;
            }
            return true;
        }

        public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn p = null, bool skipReason = false)
        {
            reason = null;
            return false;
        }
    }
}
