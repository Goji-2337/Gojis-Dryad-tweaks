using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GojisDryadTweaks
{
    [HotSwappable]
    [HarmonyPatch(typeof(CompTreeConnection), nameof(CompTreeConnection.BuildingsReducingConnectionStrength), MethodType.Getter)]
    public static class CompTreeConnection_BuildingsReducingConnectionStrength_Patch
    {
        public static bool doNotModify;
        public static void Postfix(ref List<Thing> __result, CompTreeConnection __instance)
        {
            Pawn connectedPawn = __instance.ConnectedPawn;
            if (connectedPawn == null || connectedPawn.Ideo == null)
            {
                return;
            }

            bool natureAttuned = connectedPawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_NatureAttuned);
            bool compromisedPact = connectedPawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_CompromisedPact);

            List<Thing> newResult = [.. __result];

            if (natureAttuned)
            {
                newResult.RemoveAll(thing =>
                    (thing.Stuff != null && (thing.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Woody) || thing.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)))
                );
            }

            if (doNotModify)
            {
                return;
            }
            if (compromisedPact && newResult.Count > 0)
            {
                newResult.Clear();
            }
            __result = newResult;
        }
    }
}
