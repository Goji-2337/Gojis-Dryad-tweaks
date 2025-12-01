using HarmonyLib;
using RimWorld;
using Verse;

namespace GojisDryadTweaks
{
    [HarmonyPatch(typeof(Plant), "GrowthRate", MethodType.Getter)]
    public static class Plant_GrowthRate_Patch_GauranlenBonsai
    {
        public static void Postfix(Plant __instance, ref float __result)
        {
            __result = __instance.IsGauranlenTreeInBonsaiPot() ? 0f : __result;
        }

        public static bool IsGauranlenTreeInBonsaiPot(this Thing __instance)
        {
            if (__instance.def == ThingDefOf.Plant_TreeGauranlen)
            {
                Building edifice = __instance.Position.GetEdifice(__instance.Map);
                if (edifice != null && edifice.def == GojiDefsOf.PlantPot_Bonsai)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
