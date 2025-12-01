using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(CompTreeConnection), nameof(CompTreeConnection.BuildingsReducingConnectionStrength), MethodType.Getter)]
    public static class CompTreeConnection_BuildingsReducingConnectionStrength_Patch
    {
        public static void Postfix(ref List<Thing> __result, CompTreeConnection __instance)
        {
            Pawn connectedPawn = __instance.ConnectedPawn;
            if (connectedPawn == null || connectedPawn.Ideo == null || !connectedPawn.Ideo.HasPrecept(GojiDefsOf.Goji_GauranlenConnection_NatureAttuned))
            {
                return;
            }
            __result.RemoveAll(thing =>
                (thing.Stuff != null && (thing.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Woody) || thing.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)))
            );
        }
    }
}
