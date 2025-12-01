using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks.HarmonyPatches
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    [HarmonyPatch(typeof(CompTreeConnection), nameof(CompTreeConnection.ResetDryad))]
    public static class CompTreeConnection_ResetDryad_Patch
    {
        public static Pawn pruner;
        public static void Prefix(CompTreeConnection __instance, Pawn dryad)
        {
            pruner = __instance?.ConnectedPawn;
        }
        public static void Postfix()
        {
            pruner = null;
        }
    }

    [HarmonyPatch(typeof(Pawn_TrainingTracker), nameof(Pawn_TrainingTracker.CanAssignToTrain),
    new Type[] { typeof(TrainableDef), typeof(bool) },
    new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    public static class Pawn_TrainingTracker_CanAssignToTrain_Patch
    {
        public static void Postfix(Pawn_TrainingTracker __instance, ref AcceptanceReport __result, TrainableDef td,
            ref bool visible)
        {
            var pruner = CompTreeConnection_ResetDryad_Patch.pruner;
            if (pruner != null &&
                pruner.Ideo != null &&
                pruner.Ideo.HasPrecept(GojiDefsOf.Goji_DryadAutonomy_High) &&
                td == TrainableDefOf.Obedience)
            {
                __result = true;
                Log.Message($"[GPT] Allowing training of obedience for dryad {__instance.pawn.Name} due to pruner {pruner.Name}'s high autonomy precept.");
            }
        }
    }
}
