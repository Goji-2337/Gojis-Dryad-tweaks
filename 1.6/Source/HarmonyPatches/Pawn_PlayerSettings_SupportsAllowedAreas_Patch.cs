using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(Pawn_PlayerSettings), "SupportsAllowedAreas", MethodType.Getter)]
    public static class Pawn_PlayerSettings_SupportsAllowedAreas_Patch
    {
        public static void Postfix(Pawn_PlayerSettings __instance, ref bool __result)
        {
            if (__result)
            {
                return;
            }
            if (__instance.pawn.IsDraftableControllableAnimal())
            {
                __result = true;
            }
        }
    }
}
