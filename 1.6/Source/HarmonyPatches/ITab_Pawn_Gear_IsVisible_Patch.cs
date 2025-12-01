using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "IsVisible", MethodType.Getter)]
    public static class ITab_Pawn_Gear_IsVisible_Patch
    {
        public static void Postfix(ITab_Pawn_Gear __instance, ref bool __result)
        {
            if (__instance.SelPawnForGear is Pawn pawn && pawn.IsDraftableControllableAnimal())
            {
                __result = false;
            }
        }
    }
}
