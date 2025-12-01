using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace GojisDryadTweaks
{
    public class RitualOutcomeEffectWorker_TransferConnection : RitualOutcomeEffectWorker
    {
        public RitualOutcomeEffectWorker_TransferConnection() { }

        public RitualOutcomeEffectWorker_TransferConnection(RitualOutcomeEffectDef def) : base(def) { }
        public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
        {
            var initiator = jobRitual.PawnWithRole("initiator");
            var recipient = jobRitual.PawnWithRole("recipient");
            var tree = jobRitual.selectedTarget.Thing as Plant;

            if (initiator == null || recipient == null || tree == null)
            {
                Log.Error("Goji_TransferConnection ritual outcome missing required participants or target.");
                return;
            }

            var treeComp = tree.TryGetComp<CompTreeConnection>();
            if (treeComp == null || treeComp.ConnectedPawn != initiator)
            {
                Log.Error($"Goji_TransferConnection ritual outcome: Initiator {initiator.LabelShort} is not the connected pawn of tree {tree.LabelShort}, or tree has no CompTreeConnection.");
                return;
            }
            var oldStrength = treeComp.connectionStrength;
            var oldDesiredStrength = treeComp.desiredConnectionStrength;
            var oldMode = treeComp.currentMode;
            var oldDesiredMode = treeComp.desiredMode;
            var oldDryads = new List<Pawn>(treeComp.dryads);
            initiator.connections?.ConnectedThings.Remove(tree);
            treeComp.connectedPawn = recipient;
            recipient.connections?.ConnectTo(tree);
            treeComp.connectionStrength = oldStrength;
            treeComp.desiredConnectionStrength = oldDesiredStrength;
            treeComp.currentMode = oldMode;
            treeComp.desiredMode = oldDesiredMode;
            treeComp.dryads.Clear();
            treeComp.dryads.AddRange(oldDryads);
            foreach (var dryad in treeComp.dryads)
            {
                if (treeComp.Connected && dryad.Faction != treeComp.ConnectedPawn?.Faction)
                {
                    dryad.SetFaction(treeComp.ConnectedPawn?.Faction);
                }
                if (dryad.training != null)
                {
                    foreach (TrainableDef allDef in DefDatabase<TrainableDef>.AllDefs)
                    {
                        if (dryad.training.CanAssignToTrain(allDef).Accepted)
                        {
                            dryad.training.SetWantedRecursive(allDef, checkOn: true);
                            if (allDef == TrainableDefOf.Release)
                            {
                                dryad.playerSettings.followDrafted = true;
                            }
                        }
                    }
                }
            }
            var letterText = "Goji_ConnectionTransferred_Description".Translate().Formatted(initiator.Named("INITIATOR"), recipient.Named("RECIPIENT")).CapitalizeFirst();
            Find.LetterStack.ReceiveLetter("Goji_ConnectionTransferred".Translate(), letterText, LetterDefOf.RitualOutcomePositive, new LookTargets(recipient, tree));
        }
    }
}
