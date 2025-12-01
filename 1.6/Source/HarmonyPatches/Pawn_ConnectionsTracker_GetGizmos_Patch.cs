using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(Pawn_ConnectionsTracker), nameof(Pawn_ConnectionsTracker.GetGizmos))]
    public static class Pawn_ConnectionsTracker_GetGizmos_Patch
    {
        public static bool isRunning = false;
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> gizmos)
        {
            isRunning = true;
            foreach (var g in gizmos)
            {
                yield return g;
            }
            isRunning = false;
        }
    }
}
