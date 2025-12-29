using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompTreeConnection), nameof(CompTreeConnection.BuildingsReducingConnectionStrength), MethodType.Getter)]
    public static class CompTreeConnection_BuildingsReducingConnectionStrength_Patch
    {
        public static bool doNotModify;
        public static void Postfix(List<Thing> __result, CompTreeConnection __instance)
        {
            Pawn connectedPawn = __instance.ConnectedPawn;
            if (connectedPawn == null || connectedPawn.Ideo == null)
            {
                return;
            }

            bool natureAttuned = connectedPawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_NatureAttuned);
            bool compromisedPact = connectedPawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_CompromisedPact);

            if (natureAttuned)
            {
                __result.RemoveAll(thing =>
                    (thing.Stuff != null && (thing.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Woody) || thing.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)))
                );
            }

            if (doNotModify)
            {
                return;
            }
            if (compromisedPact && __result.Count > 0)
            {
                __result.Clear();
            }
        }
    }
}
