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

            var connectedComps = new List<CompTreeConnection>();
            foreach (var connectedThing in initiator.connections?.ConnectedThings ?? Enumerable.Empty<Thing>())
            {
                var comp = connectedThing.TryGetComp<CompTreeConnection>();
                if (comp != null)
                {
                    connectedComps.Add(comp);
                }
            }

            foreach (var comp in connectedComps)
            {
                if (comp.ConnectedPawn != initiator)
                {
                    continue;
                }
                var oldStrength = comp.connectionStrength;
                var oldDesiredStrength = comp.desiredConnectionStrength;
                var oldMode = comp.currentMode;
                var oldDesiredMode = comp.desiredMode;
                var oldDryads = new List<Pawn>(comp.dryads);
                initiator.connections?.ConnectedThings.Remove(comp.parent);
                comp.connectedPawn = recipient;
                recipient.connections?.ConnectTo(comp.parent);
                comp.connectionStrength = oldStrength;
                comp.desiredConnectionStrength = oldDesiredStrength;
                comp.currentMode = oldMode;
                comp.desiredMode = oldDesiredMode;
                comp.dryads.Clear();
                comp.dryads.AddRange(oldDryads);
                foreach (var dryad in comp.dryads)
                {
                    if (comp.Connected && dryad.Faction != comp.ConnectedPawn?.Faction)
                    {
                        dryad.SetFaction(comp.ConnectedPawn?.Faction);
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
                    if (dryad.relations != null && initiator.relations != null && recipient.relations != null)
                    {
                        if (dryad.relations.DirectRelationExists(PawnRelationDefOf.Bond, initiator))
                        {
                            dryad.relations.RemoveDirectRelation(PawnRelationDefOf.Bond, initiator);
                        }
                        if (!dryad.relations.DirectRelationExists(PawnRelationDefOf.Bond, recipient))
                        {
                            dryad.playerSettings.Master = recipient;
                            recipient.relations.AddDirectRelation(PawnRelationDefOf.Bond, dryad);
                        }
                    }
                }
            }
            var letterText = "Goji_ConnectionTransferred_Description".Translate().Formatted(initiator.Named("INITIATOR"), recipient.Named("RECIPIENT")).CapitalizeFirst();
            Find.LetterStack.ReceiveLetter("Goji_ConnectionTransferred".Translate(), letterText, LetterDefOf.RitualOutcomePositive, new LookTargets(recipient, tree));
        }
    }
}
