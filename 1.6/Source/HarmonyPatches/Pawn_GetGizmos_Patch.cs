using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.AI;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Pawn_GetGizmos_Patch
    {
        public static bool IsDraftableControllableAnimal(this Pawn pawn)
        {
            if (pawn.RaceProps != null && pawn.RaceProps.Dryad && pawn.Faction == Faction.OfPlayer)
            {
                Thing tree = pawn.connections?.ConnectedThings.FirstOrDefault(t => t.def == ThingDefOf.Plant_TreeGauranlen);
                CompTreeConnection treeComp = tree?.TryGetComp<CompTreeConnection>();
                Pawn pruner = treeComp?.ConnectedPawn;
                if (pruner?.Ideo != null && pruner.Ideo.HasPrecept(GojiDefsOf.Goji_DryadAutonomy_High))
                {
                    return true;
                }
                else if (pawn.Drafted)
                {
                    pawn.drafter.Drafted = false;
                }
            }
            return false;
        }

        [HarmonyPriority(int.MaxValue)]

        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            var pawn = __instance;
            bool alreadyHasVanillaDraftButton = false;
            foreach (var g in __result)
            {
                if (g is Command_Toggle command && command.defaultDesc == "CommandToggleDraftDesc".Translate())
                {
                    alreadyHasVanillaDraftButton = true;
                }
                yield return g;
            }

            if (!alreadyHasVanillaDraftButton && pawn.IsDraftableControllableAnimal())
            {
                pawn.drafter ??= new Pawn_DraftController(pawn);
                pawn.pather ??= new Pawn_PathFollower(pawn);
                Command_Toggle drafting_command = new Command_Toggle();
                drafting_command.toggleAction = delegate
                {
                    pawn.drafter.Drafted = !pawn.drafter.Drafted;
                };
                drafting_command.isActive = (() => pawn.drafter.Drafted);
                drafting_command.defaultLabel = (pawn.drafter.Drafted ? "CommandUndraftLabel" : "CommandDraftLabel").Translate();
                drafting_command.hotKey = KeyBindingDefOf.Command_ColonistDraft;
                drafting_command.defaultDesc = "CommandToggleDraftDesc".Translate();
                drafting_command.icon = ContentFinder<Texture2D>.Get("ui/commands/Draft", true);
                drafting_command.turnOnSound = SoundDefOf.DraftOn;
                drafting_command.groupKey = 81729172;
                drafting_command.turnOffSound = SoundDefOf.DraftOff;
                yield return drafting_command;
            }
        }
    }
}
