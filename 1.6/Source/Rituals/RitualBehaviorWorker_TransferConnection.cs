using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace GojisDryadTweaks
{
    public class RitualBehaviorWorker_TransferConnection : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_TransferConnection() { }

        public RitualBehaviorWorker_TransferConnection(RitualBehaviorDef def) : base(def) { }
        public override string CanStartRitualNow(TargetInfo target, Precept_Ritual ritual, Pawn selectedPawn = null, Dictionary<string, Pawn> forcedForRole = null)
        {
            if (!target.HasThing || !(target.Thing is Plant))
            {
                return "RitualTargetNotTree".Translate();
            }
            var treeComp = target.Thing.TryGetComp<CompTreeConnection>();
            if (treeComp == null)
            {
                return "RitualTargetNotGauranlenTree".Translate();
            }
            if (!treeComp.Connected)
            {
                return "Goji_RitualTargetMustBeConnectedGauranlenTree".Translate();
            }
            if (treeComp.ConnectionTorn)
            {
                return "RitualTargetConnectionTornGauranlenTree".Translate(target.Thing.Named("TREE"), treeComp.UntornInDurationTicks.ToStringTicksToPeriod()).Resolve().CapitalizeFirst();
            }
            var initiator = treeComp.ConnectedPawn;
            if (initiator == null)
            {
                return "Goji_RitualTargetMustBeConnectedGauranlenTree".Translate();
            }
            if (initiator.Dead || initiator.Downed || initiator.IsPrisoner || initiator.IsSlave)
            {
                return "MessageRitualPawnUnavailable".Translate(initiator.LabelShort);
            }
            if (!AbilityUtility.ValidateNoMentalState(initiator, false, null))
            {
                return "MessageRitualPawnUnavailable".Translate(initiator.LabelShort);
            }
            if (!AbilityUtility.ValidateCanWalk(initiator, false, null))
            {
                return "MessageRitualPawnUnavailable".Translate(initiator.LabelShort);
            }
            bool foundValidRecipient = false;
            foreach (var p in target.Map.mapPawns.FreeColonistsSpawned)
            {
                if (ValidateRecipient(p, initiator, false))
                {
                    foundValidRecipient = true;
                    break;
                }
            }

            if (!foundValidRecipient)
            {
                return "Goji_NoValidRecipientFound".Translate();
            }
            return base.CanStartRitualNow(target, ritual, selectedPawn, forcedForRole);
        }
        public static bool ValidateRecipient(Pawn recipient, Pawn initiator, bool throwMessages)
        {
            if (recipient == null || recipient == initiator)
            {
                return false;
            }
            if (recipient.Ideo != initiator.Ideo)
            {
                if (throwMessages) Messages.Message("MessageRitualRequiresSameIdeoligion".Translate(initiator.Named("INITIATOR"), recipient.Named("RECIPIENT")), recipient, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            if (recipient.connections?.ConnectedThings.Any(t => t.TryGetComp<CompTreeConnection>() != null) ?? false)
            {
                if (throwMessages) Messages.Message("Goji_RecipientAlreadyConnected".Translate(recipient.Named("PAWN")), recipient, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }

            if (!AbilityUtility.ValidateNoMentalState(recipient, throwMessages, null))
            {
                return false;
            }
            if (!AbilityUtility.ValidateCanWalk(recipient, throwMessages, null))
            {
                return false;
            }

            return true;
        }
    }
}
