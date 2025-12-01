using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System;

namespace GojisDryadTweaks
{
    [HarmonyPatch]
    public static class CompSummonCompanionDryads_Apply_Patch
    {
        public static MethodBase targetMethod;
        public static bool Prepare()
        {
            if (!ModsConfig.IsActive("vanillaracesexpanded.phytokin"))
            {
                return false;
            }
            var type = AccessTools.TypeByName("VanillaRacesExpandedPhytokin.CompSummonCompanionDryads");
            if (type == null)
            {
                Log.Error("Goji's Personal Tweaks: Could not find VanillaRacesExpandedPhytokin.CompSummonCompanionDryads type.");
                return false;
            }
            targetMethod = AccessTools.Method(type, "Apply", new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo) });
            if (targetMethod == null)
            {
                Log.Error("Goji's Personal Tweaks: Could not find VanillaRacesExpandedPhytokin.CompSummonCompanionDryads.Apply method.");
                return false;
            }
            return true;
        }

        public static MethodBase TargetMethod()
        {
            return targetMethod;
        }

        public static bool Prefix(object __instance, LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!(__instance is CompAbilityEffect abilityComp))
            {
                return true;
            }
            var casterPawn = abilityComp.parent?.pawn;
            if (casterPawn == null)
            {
                return true;
            }
            if (GojiDefsOf.VRE_GauranlenAffinity != null && casterPawn.genes != null && casterPawn.genes.HasActiveGene(GojiDefsOf.VRE_GauranlenAffinity))
            {
                var companionDryadDef = GojiDefsOf.VRE_CompanionDryad;

                var dryadTypes = DefDatabase<PawnKindDef>.AllDefsListForReading
                    .Where(pk => pk.RaceProps != null && pk.RaceProps.Dryad && pk != companionDryadDef)
                    .ToList();

                if (dryadTypes.NullOrEmpty())
                {
                    Log.Warning("Goji's Personal Tweaks: No standard dryad types found to summon.");
                    return false;
                }

                var options = new List<FloatMenuOption>();
                foreach (var dryadType in dryadTypes)
                {
                    options.Add(new FloatMenuOption(dryadType.LabelCap, delegate
                    {
                        SpawnChosenDryad(casterPawn, dryadType, target.Cell);
                    }, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
                }

                Find.WindowStack.Add(new FloatMenu(options));

                return false;
            }

            return true;
        }

        private static void SpawnChosenDryad(Pawn caster, PawnKindDef dryadKind, IntVec3 cell)
        {
            var dryad = PawnGenerator.GeneratePawn(dryadKind, caster.Faction);
            GenSpawn.Spawn(dryad, cell, caster.Map, Rot4.South);
        }
    }
}
