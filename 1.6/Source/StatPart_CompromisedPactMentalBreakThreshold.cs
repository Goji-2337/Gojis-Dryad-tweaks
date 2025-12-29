using System.Linq;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [HotSwappable]
    public class StatPart_CompromisedPactMentalBreakThreshold : StatPart
    {
        private const float MentalBreakThresholdBonus = 0.08f;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing && req.Thing is Pawn pawn)
            {
                if (pawn.Ideo != null && pawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_CompromisedPact))
                {
                    if (HasAffectedTree(pawn))
                    {
                        val += MentalBreakThresholdBonus;
                    }
                }
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing && req.Thing is Pawn pawn)
            {
                if (pawn.Ideo != null && pawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_CompromisedPact))
                {
                    if (HasAffectedTree(pawn))
                    {
                        return "Goji_CompromisedPactMentalBreakThreshold".Translate() + ": +" + MentalBreakThresholdBonus.ToStringPercent();
                    }
                }
            }
            return null;
        }

        private bool HasAffectedTree(Pawn pawn)
        {
            if (pawn.connections == null)
            {
                return false;
            }
            CompTreeConnection_BuildingsReducingConnectionStrength_Patch.doNotModify = true;
            var result = pawn.connections.ConnectedThings.Any(thing =>
            {
                if (thing is Plant plant && plant.TryGetComp(out CompTreeConnection comp))
                {
                    return comp.Connected && comp.BuildingsReducingConnectionStrength.Any();
                }
                return false;
            });
            CompTreeConnection_BuildingsReducingConnectionStrength_Patch.doNotModify = false;
            return result;
        }
    }
}
